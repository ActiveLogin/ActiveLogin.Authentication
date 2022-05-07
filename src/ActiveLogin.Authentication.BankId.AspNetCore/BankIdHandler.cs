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

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ActiveLogin.Authentication.BankId.AspNetCore;

public class BankIdHandler : RemoteAuthenticationHandler<BankIdOptions>
{
    private const string StateCookieNameParemeterName = "StateCookie.Name";

    private readonly IBankIdUiStateProtector _uiStateProtector;
    private readonly IBankIdUiOptionsProtector _uiOptionsProtector;
    private readonly IBankIdUiAuthResultProtector _uiResultProtector;
    private readonly IBankIdEventTrigger _bankIdEventTrigger;
    private readonly IBankIdSupportedDeviceDetector _bankIdSupportedDeviceDetector;
    private readonly List<IBankIdClaimsTransformer> _bankIdClaimsTransformers;

    public BankIdHandler(
        IOptionsMonitor<BankIdOptions> options,
        ILoggerFactory loggerFactory,
        UrlEncoder encoder,
        ISystemClock clock,
        IBankIdUiStateProtector uiStateProtector,
        IBankIdUiOptionsProtector uiOptionsProtector,
        IBankIdUiAuthResultProtector uiResultProtector,
        IBankIdEventTrigger bankIdEventTrigger,
        IBankIdSupportedDeviceDetector bankIdSupportedDeviceDetector,
        IEnumerable<IBankIdClaimsTransformer> bankIdClaimsTransformers)
        : base(options, loggerFactory, encoder, clock)
    {
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

        var protectedUiResult = Request.Query[BankIdConstants.QueryStringParameters.UiResult];
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

    private async Task<AuthenticationTicket> GetAuthenticationTicket(BankIdUiAuthResult uiAuthResult, AuthenticationProperties properties)
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

    private async Task<IEnumerable<Claim>> GetClaims(BankIdUiAuthResult uiAuthResult)
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
            GetCancelReturnUrl(properties),
            Options.StateCookie.Name ?? string.Empty
        );

        var detectedDevice = _bankIdSupportedDeviceDetector.Detect();
        await _bankIdEventTrigger.TriggerAsync(new BankIdAspNetChallengeSuccessEvent(detectedDevice, uiOptions.ToBankIdFlowOptions()));

        var loginUrl = GetLoginUrl(uiOptions);
        Response.Redirect(loginUrl);
    }

    private string GetCancelReturnUrl(AuthenticationProperties properties)
    {
        // Default to root if no return url is set
        var cancelReturnUrl = properties.Items.ContainsKey(BankIdConstants.QueryStringParameters.ReturnUrl)
                                        ? properties.Items[BankIdConstants.QueryStringParameters.ReturnUrl]
                                        : BankIdConstants.DefaultCancelUrl;

        // If cancel url is set, it overrides the regular return url
        if (properties.Items.TryGetValue(BankIdConstants.AuthenticationPropertiesKeys.CancelReturnUrl, out var cancelUrl))
        {
            cancelReturnUrl = cancelUrl;
        }

        return cancelReturnUrl ?? BankIdConstants.DefaultCancelUrl;
    }

    private string GetLoginUrl(BankIdUiOptions uiOptions)
    {
        var pathBase = Context.Request.PathBase;
        var loginUrl = pathBase.Add(Options.LoginPath);
        var returnUrl = pathBase.Add(Options.CallbackPath);
        var protectedUiOptions = _uiOptionsProtector.Protect(uiOptions);

        var queryBuilder = new QueryBuilder(new Dictionary<string, string>
        {
            { BankIdConstants.QueryStringParameters.ReturnUrl, returnUrl },
            { BankIdConstants.QueryStringParameters.UiOptions, protectedUiOptions }
        });

        return $"{loginUrl}{queryBuilder.ToQueryString()}";
    }

    private void AppendStateCookie(AuthenticationProperties properties)
    {
        Validators.ThrowIfNullOrWhitespace(Options.StateCookie.Name, StateCookieNameParemeterName);

        var state = new BankIdUiState(properties);
        var cookieOptions = Options.StateCookie.Build(Context, Clock.UtcNow);
        var cookieValue = _uiStateProtector.Protect(state);

        Response.Cookies.Append(Options.StateCookie.Name, cookieValue, cookieOptions);
    }

    private BankIdUiState? GetStateFromCookie()
    {
        Validators.ThrowIfNullOrWhitespace(Options.StateCookie.Name, StateCookieNameParemeterName);

        var protectedState = Request.Cookies[Options.StateCookie.Name];
        if (string.IsNullOrEmpty(protectedState))
        {
            return null;
        }

        var state = _uiStateProtector.Unprotect(protectedState);
        return state;
    }

    private void DeleteStateCookie()
    {
        Validators.ThrowIfNullOrWhitespace(Options.StateCookie.Name, StateCookieNameParemeterName);

        var cookieOptions = Options.StateCookie.Build(Context, Clock.UtcNow);
        Response.Cookies.Delete(Options.StateCookie.Name, cookieOptions);
    }
}
