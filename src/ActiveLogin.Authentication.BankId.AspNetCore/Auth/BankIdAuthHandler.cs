using System.Security.Claims;
using System.Text.Encodings.Web;

using ActiveLogin.Authentication.BankId.AspNetCore.ClaimsTransformation;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Helpers;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.Core;
using ActiveLogin.Authentication.BankId.Core.Events;
using ActiveLogin.Authentication.BankId.Core.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.Core.SupportedDevice;
using ActiveLogin.Identity.Swedish;

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Auth;

public class BankIdAuthHandler(
    IHttpContextAccessor httpContextAccessor,
    IAntiforgery antiforgery,
    IOptionsMonitor<BankIdAuthOptions> options,
    ILoggerFactory loggerFactory,
    UrlEncoder encoder,
    IBankIdDataStateProtector<BankIdUiOptions> uiOptionsProtector,
    IBankIdDataStateProtector<BankIdUiResult> uiResultProtector,
    IBankIdEventTrigger bankIdEventTrigger,
    IBankIdSupportedDeviceDetector bankIdSupportedDeviceDetector,
    IEnumerable<IBankIdClaimsTransformer> bankIdClaimsTransformers,
    IStateStorage stateStorage
) : RemoteAuthenticationHandler<BankIdAuthOptions>(options, loggerFactory, encoder)
{
    private readonly PathString _authPath = new($"/{BankIdConstants.Routes.ActiveLoginAreaName}/{BankIdConstants.Routes.BankIdPathName}/{BankIdConstants.Routes.BankIdAuthControllerPath}");
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IAntiforgery _antiforgery = antiforgery;
    private readonly IStateStorage _stateStorage = stateStorage;
    private readonly IBankIdDataStateProtector<BankIdUiOptions> _uiOptionsProtector = uiOptionsProtector;
    private readonly IBankIdDataStateProtector<BankIdUiResult> _uiResultProtector = uiResultProtector;
    private readonly IBankIdEventTrigger _bankIdEventTrigger = bankIdEventTrigger;
    private readonly IBankIdSupportedDeviceDetector _bankIdSupportedDeviceDetector = bankIdSupportedDeviceDetector;
    private readonly List<IBankIdClaimsTransformer> _bankIdClaimsTransformers = bankIdClaimsTransformers.ToList();

    protected override async Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
    {
        var detectedDevice = _bankIdSupportedDeviceDetector.Detect();

        var state = await GetStateFromCookie();
        if (state == null)
        {
            return await HandleRemoteAuthenticateFail(BankIdConstants.ErrorMessages.InvalidStateCookie, detectedDevice);
        }

        DeleteStateCookie();

        if (!Request.HasFormContentType)
        {
            await HandleRemoteAuthenticateFail(BankIdConstants.ErrorMessages.InvalidUiResult, detectedDevice);
            throw new ArgumentException(BankIdConstants.ErrorMessages.InvalidUiResult);
        }

        var httpContext = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException(BankIdConstants.ErrorMessages.CouldNotAccessHttpContext);
        await _antiforgery.ValidateRequestAsync(httpContext);

        var protectedUiResult = Request.Form[BankIdConstants.FormParameters.UiResult];
        if (StringValues.IsNullOrEmpty(protectedUiResult))
        {
            return await HandleRemoteAuthenticateFail(BankIdConstants.ErrorMessages.InvalidUiResult, detectedDevice);
        }

        var uiResult = _uiResultProtector.Unprotect(protectedUiResult.ToString());
        if (!uiResult.IsSuccessful)
        {
            return await HandleRemoteAuthenticateFail(BankIdConstants.ErrorMessages.InvalidUiResult, detectedDevice);
        }

        var properties = state.AuthenticationProperties;
        var ticket = await GetAuthenticationTicket(uiResult, properties);

        await _bankIdEventTrigger.TriggerAsync(new BankIdAspNetAuthenticateSuccessEvent(
            PersonalIdentityNumber.Parse(uiResult.PersonalIdentityNumber),
            detectedDevice
        ));

        return HandleRequestResult.Success(ticket);
    }

    private async Task<HandleRequestResult> HandleRemoteAuthenticateFail(string reason, BankIdSupportedDevice detectedDevice)
    {
        await _bankIdEventTrigger.TriggerAsync(new BankIdAspNetAuthenticateFailureEvent(reason, detectedDevice));

        return HandleRequestResult.Fail(reason);
    }

    private async Task<AuthenticationTicket> GetAuthenticationTicket(BankIdUiResult uiAuthResult, AuthenticationProperties properties)
    {
        if (Options.TokenExpiresIn.HasValue)
        {
            if (Options.TimeProvider == null)
            {
                throw new InvalidOperationException(BankIdConstants.ErrorMessages.TimeProviderNotSet);
            }

            properties.ExpiresUtc = Options.TimeProvider?.GetUtcNow().Add(Options.TokenExpiresIn.Value);
        }

        var claims = await GetClaims(uiAuthResult);
        var identity = new ClaimsIdentity(claims, Scheme.Name, BankIdClaimTypes.Name, BankIdClaimTypes.Role);
        var principal = new ClaimsPrincipal(identity);

        return new AuthenticationTicket(principal, properties, Scheme.Name);
    }

    private async Task<IEnumerable<Claim>> GetClaims(BankIdUiResult uiAuthResult)
    {
        var context = new BankIdClaimsTransformationContext(
            Options,

            uiAuthResult.BankIdOrderRef,

            uiAuthResult.PersonalIdentityNumber,

            uiAuthResult.Name,
            uiAuthResult.GivenName,
            uiAuthResult.Surname,

            uiAuthResult.GetCompletionData()
        );

        foreach (var transformer in _bankIdClaimsTransformers)
        {
            await transformer.TransformClaims(context);
        }

        return context.Claims;
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        await AppendStateCookie(properties);

        var uiOptions = new BankIdUiOptions(
            Options.BankIdCertificatePolicies,
            Options.BankIdSameDevice,
            Options.BankIdRequirePinCode,
            Options.BankIdRequireMrtd,
            Options.BankIdReturnRisk,
            BankIdHandlerHelper.GetCancelReturnUrl(properties.Items),
            Options.StateCookie.Name ?? string.Empty,
            Options.CardReader
        );

        var detectedDevice = _bankIdSupportedDeviceDetector.Detect();
        await _bankIdEventTrigger.TriggerAsync(new BankIdAspNetChallengeSuccessEvent(detectedDevice, uiOptions.ToBankIdFlowOptions()));

        var loginUrl = GetInitUiUrl(uiOptions);
        Response.Redirect(loginUrl);
    }

    private string GetInitUiUrl(BankIdUiOptions uiOptions)
    {
        var pathBase = Context.Request.PathBase;
        var authUrl = pathBase.Add(_authPath);
        var returnUrl = pathBase.Add(Options.CallbackPath);
        var protectedUiOptions = _uiOptionsProtector.Protect(uiOptions);

        var queryBuilder = new QueryBuilder(new Dictionary<string, string>
        {
            { BankIdConstants.QueryStringParameters.ReturnUrl, returnUrl },
            { BankIdConstants.QueryStringParameters.UiOptions, protectedUiOptions }
        });

        return $"{authUrl}{queryBuilder.ToQueryString()}";
    }

    private async Task AppendStateCookie(AuthenticationProperties properties)
    {
        Validators.ThrowIfNullOrWhitespace(Options.StateCookie.Name, BankIdConstants.AuthStateKey);

        if (Options.TimeProvider == null)
        {
            throw new InvalidOperationException(BankIdConstants.ErrorMessages.TimeProviderNotSet);
        }

        var state = new BankIdUiAuthState(properties);
        var stateKey = await _stateStorage.SetAsync(state);

        var cookieOptions = Options.StateCookie.Build(Context, Options.TimeProvider.GetUtcNow());
        Response.Cookies.Append(Options.StateCookie.Name, stateKey, cookieOptions);
    }

    private Task<BankIdUiAuthState?> GetStateFromCookie()
    {
        Validators.ThrowIfNullOrWhitespace(Options.StateCookie.Name, BankIdConstants.AuthStateKey);

        var stateKey = Request.Cookies[Options.StateCookie.Name];
        if (string.IsNullOrEmpty(stateKey))
        {
            return Task.FromResult<BankIdUiAuthState?>(null);
        }

        return _stateStorage.GetAsync<BankIdUiAuthState>(new(stateKey));
    }

    private void DeleteStateCookie()
    {
        Validators.ThrowIfNullOrWhitespace(Options.StateCookie.Name, BankIdConstants.AuthStateKey);

        if (Options.TimeProvider == null)
        {
            throw new InvalidOperationException(BankIdConstants.ErrorMessages.TimeProviderNotSet);
        }

        var cookieOptions = Options.StateCookie.Build(Context, Options.TimeProvider.GetUtcNow());
        Response.Cookies.Delete(Options.StateCookie.Name, cookieOptions);
    }
}
