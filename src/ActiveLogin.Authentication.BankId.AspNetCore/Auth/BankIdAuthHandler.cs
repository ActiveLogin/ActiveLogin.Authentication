using System.Security.Claims;
using System.Text.Encodings.Web;

using ActiveLogin.Authentication.BankId.AspNetCore.ClaimsTransformation;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Helpers;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
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

namespace ActiveLogin.Authentication.BankId.AspNetCore.Auth;

public class BankIdAuthHandler : RemoteAuthenticationHandler<BankIdAuthOptions>
{
    private const string StateCookieNameParameterName = "StateCookie.Name";
    private readonly PathString _authPath = new($"/{BankIdConstants.Routes.ActiveLoginAreaName}/{BankIdConstants.Routes.BankIdPathName}/{BankIdConstants.Routes.BankIdAuthControllerPath}");

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAntiforgery _antiforgery;
    private readonly IBankIdUiStateProtector _uiStateProtector;
    private readonly IBankIdUiOptionsProtector _uiOptionsProtector;
    private readonly IBankIdUiResultProtector _uiResultProtector;
    private readonly IBankIdEventTrigger _bankIdEventTrigger;
    private readonly IBankIdSupportedDeviceDetector _bankIdSupportedDeviceDetector;
    private readonly List<IBankIdClaimsTransformer> _bankIdClaimsTransformers;

    public BankIdAuthHandler(
        IHttpContextAccessor httpContextAccessor,
        IAntiforgery antiforgery,
        IOptionsMonitor<BankIdAuthOptions> options,
        ILoggerFactory loggerFactory,
        UrlEncoder encoder,
        ISystemClock clock,
        IBankIdUiStateProtector uiStateProtector,
        IBankIdUiOptionsProtector uiOptionsProtector,
        IBankIdUiResultProtector uiResultProtector,
        IBankIdEventTrigger bankIdEventTrigger,
        IBankIdSupportedDeviceDetector bankIdSupportedDeviceDetector,
        IEnumerable<IBankIdClaimsTransformer> bankIdClaimsTransformers)
        : base(options, loggerFactory, encoder, clock)
    {
        _httpContextAccessor = httpContextAccessor;
        _antiforgery = antiforgery;
        _uiStateProtector = uiStateProtector;
        _uiOptionsProtector = uiOptionsProtector;
        _uiResultProtector = uiResultProtector;
        _bankIdEventTrigger = bankIdEventTrigger;
        _bankIdSupportedDeviceDetector = bankIdSupportedDeviceDetector;
        _bankIdClaimsTransformers = bankIdClaimsTransformers.ToList();
    }

    protected override async Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
    {
        var detectedDevice = _bankIdSupportedDeviceDetector.Detect();

        var state = GetStateFromCookie();
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

        var protectedUiResult = Request.Form[BankIdConstants.QueryStringParameters.UiResult];
        if (string.IsNullOrEmpty(protectedUiResult))
        {
            return await HandleRemoteAuthenticateFail(BankIdConstants.ErrorMessages.InvalidUiResult, detectedDevice);
        }

        var uiResult = _uiResultProtector.Unprotect(protectedUiResult);
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
            properties.ExpiresUtc = Clock.UtcNow.Add(Options.TokenExpiresIn.Value);
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
            uiAuthResult.Surname
        );

        foreach (var transformer in _bankIdClaimsTransformers)
        {
            await transformer.TransformClaims(context);
        }

        return context.Claims;
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        AppendStateCookie(properties);

        var uiOptions = new BankIdUiOptions(
            Options.BankIdCertificatePolicies,
            Options.BankIdSameDevice,
            Options.BankIdAllowBiometric,
            BankIdHandlerHelper.GetCancelReturnUrl(properties.Items),
            Options.StateCookie.Name ?? string.Empty
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

    private void AppendStateCookie(AuthenticationProperties properties)
    {
        Validators.ThrowIfNullOrWhitespace(Options.StateCookie.Name, StateCookieNameParameterName);

        var state = new BankIdUiAuthState(properties);
        var cookieOptions = Options.StateCookie.Build(Context, Clock.UtcNow);
        var cookieValue = _uiStateProtector.Protect(state);

        Response.Cookies.Append(Options.StateCookie.Name, cookieValue, cookieOptions);
    }

    private BankIdUiAuthState? GetStateFromCookie()
    {
        Validators.ThrowIfNullOrWhitespace(Options.StateCookie.Name, StateCookieNameParameterName);

        var protectedState = Request.Cookies[Options.StateCookie.Name];
        if (string.IsNullOrEmpty(protectedState))
        {
            return null;
        }

        return _uiStateProtector.Unprotect(protectedState) as BankIdUiAuthState;
    }

    private void DeleteStateCookie()
    {
        Validators.ThrowIfNullOrWhitespace(Options.StateCookie.Name, StateCookieNameParameterName);

        var cookieOptions = Options.StateCookie.Build(Context, Clock.UtcNow);
        Response.Cookies.Delete(Options.StateCookie.Name, cookieOptions);
    }
}
