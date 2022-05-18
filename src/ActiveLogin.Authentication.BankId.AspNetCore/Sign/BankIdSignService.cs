using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Helpers;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.Core.Events;
using ActiveLogin.Authentication.BankId.Core.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.Core.Flow;
using ActiveLogin.Authentication.BankId.Core.Helpers;
using ActiveLogin.Authentication.BankId.Core.SupportedDevice;
using ActiveLogin.Identity.Swedish;

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Sign;

public class BankIdSignService : IBankIdSignService
{
    private const string StateCookieNameParameterName = "StateCookie.Name";
    private readonly PathString _signInitPath = new($"/{BankIdConstants.Routes.ActiveLoginAreaName}/{BankIdConstants.Routes.BankIdPathName}/{BankIdConstants.Routes.BankIdSignControllerPath}");

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAntiforgery _antiforgery;

    private readonly IOptionsSnapshot<BankIdSignOptions> _optionsSnapshot;
    private readonly IBankIdFlowSystemClock _systemClock;
    private readonly IBankIdUiStateProtector _bankIdUiStateProtector;
    private readonly IBankIdUiResultProtector _uiResultProtector;
    private readonly IBankIdUiOptionsProtector _uiOptionsProtector;
    private readonly IBankIdSupportedDeviceDetector _bankIdSupportedDeviceDetector;
    private readonly IBankIdEventTrigger _bankIdEventTrigger;

    public BankIdSignService(
        IHttpContextAccessor httpContextAccessor,
        IAntiforgery antiforgery,
        IOptionsSnapshot<BankIdSignOptions> optionsSnapshot,
        IBankIdFlowSystemClock systemClock,
        IBankIdUiStateProtector bankIdUiStateProtector,
        IBankIdUiOptionsProtector uiOptionsProtector,
        IBankIdSupportedDeviceDetector bankIdSupportedDeviceDetector,
        IBankIdEventTrigger bankIdEventTrigger,
        IBankIdUiResultProtector uiResultProtector)
    {
        _httpContextAccessor = httpContextAccessor;
        _antiforgery = antiforgery;
        _optionsSnapshot = optionsSnapshot;
        _systemClock = systemClock;
        _bankIdUiStateProtector = bankIdUiStateProtector;
        _uiOptionsProtector = uiOptionsProtector;
        _bankIdSupportedDeviceDetector = bankIdSupportedDeviceDetector;
        _bankIdEventTrigger = bankIdEventTrigger;
        _uiResultProtector = uiResultProtector;
    }

    public IActionResult InitiateSign(BankIdSignProperties properties, string callbackPath, string configKey)
    {
        var httpContext = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("Can't access HttpContext");
        var options = _optionsSnapshot.Get(configKey);

        AppendStateCookie(httpContext, properties, options, configKey);

        var uiOptions = new BankIdUiOptions(
            options.BankIdCertificatePolicies,
            options.BankIdSameDevice,
            options.BankIdAllowBiometric,
            BankIdHandlerHelper.GetCancelReturnUrl(properties.Items),
            options.StateCookie.Name ?? string.Empty
        );

        var signUrl = GetUiInitUrl(httpContext, callbackPath, uiOptions);
        return new RedirectResult(signUrl);
    }

    public async Task<BankIdSignResult?> GetSignResultAsync(string provider)
    {
        Validators.ThrowIfNullOrWhitespace(provider, nameof(provider));

        var detectedDevice = _bankIdSupportedDeviceDetector.Detect();
        var options = _optionsSnapshot.Get(provider);
        var httpContext = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("Can't access HttpContext");

        var state = GetStateCookie(httpContext, options);
        if(state is not BankIdUiSignState signState)
        {
            await _bankIdEventTrigger.TriggerAsync(new BankIdSignFailureEvent(BankIdConstants.ErrorMessages.InvalidStateCookie, detectedDevice));
            throw new ArgumentException($"Missing or invalid state cookie {options.StateCookie.Name}");
        }

        DeleteStateCookie(httpContext, options);

        if (!httpContext.Request.HasFormContentType)
        {
            await _bankIdEventTrigger.TriggerAsync(new BankIdSignFailureEvent(BankIdConstants.ErrorMessages.InvalidUiResult, detectedDevice));
            throw new ArgumentException($"Missing or invalid ui result");
        }

        await _antiforgery.ValidateRequestAsync(httpContext);

        var protectedUiResult = httpContext.Request.Form[BankIdConstants.QueryStringParameters.UiResult];
        if (string.IsNullOrEmpty(protectedUiResult))
        {
            await _bankIdEventTrigger.TriggerAsync(new BankIdSignFailureEvent(BankIdConstants.ErrorMessages.InvalidUiResult, detectedDevice));
            return new BankIdSignResult(false, signState.BankIdSignProperties);
        }

        var uiResult = _uiResultProtector.Unprotect(protectedUiResult);
        if (!uiResult.IsSuccessful)
        {
            await _bankIdEventTrigger.TriggerAsync(new BankIdSignFailureEvent(BankIdConstants.ErrorMessages.InvalidUiResult, detectedDevice));
            return new BankIdSignResult(false, signState.BankIdSignProperties);
        }

        await _bankIdEventTrigger.TriggerAsync(new BankIdSignSuccessEvent(
            PersonalIdentityNumber.Parse(uiResult.PersonalIdentityNumber),
            detectedDevice
        ));

        return BankIdSignResult.Success(
            signState.BankIdSignProperties,
            new CompletionData(
                ParseUser(uiResult),
                ParseDevice(uiResult),
                ParseCert(uiResult),
                uiResult.Signature,
                uiResult.OCSPResponse)
            );
    }

    private BankIdUiState GetStateCookie(HttpContext httpContext, BankIdSignOptions options)
    {
        Validators.ThrowIfNullOrWhitespace(options.StateCookie.Name, StateCookieNameParameterName);

        var protectedState = httpContext.Request.Cookies[options.StateCookie.Name];
        if (string.IsNullOrEmpty(protectedState))
        {
            throw new InvalidOperationException($"Missing state cookie {options.StateCookie.Name}");
        }
        return _bankIdUiStateProtector.Unprotect(protectedState);
    }

    private void AppendStateCookie(HttpContext httpContext, BankIdSignProperties properties, BankIdSignOptions options, string configKey)
    {
        Validators.ThrowIfNullOrWhitespace(options.StateCookie.Name, StateCookieNameParameterName);

        var state = new BankIdUiSignState(configKey, properties);
        var cookieOptions = options.StateCookie.Build(httpContext, _systemClock.UtcNow);
        var cookieValue = _bankIdUiStateProtector.Protect(state);

        httpContext.Response.Cookies.Append(options.StateCookie.Name, cookieValue, cookieOptions);
    }

    private string GetUiInitUrl(HttpContext httpContext, string callbackPath, BankIdUiOptions uiOptions)
    {
        var pathBase = httpContext.Request.PathBase;
        var signUrl = pathBase.Add(_signInitPath);

        var protectedUiOptions = _uiOptionsProtector.Protect(uiOptions);

        var queryBuilder = QueryStringGenerator.ToQueryString(
            new Dictionary<string, string>
            {
                { BankIdConstants.QueryStringParameters.ReturnUrl, callbackPath },
                { BankIdConstants.QueryStringParameters.UiOptions, protectedUiOptions }
            });

        return $"{signUrl}{queryBuilder}";
    }

    private void DeleteStateCookie(HttpContext httpContext, BankIdSignOptions options)
    {
        Validators.ThrowIfNullOrWhitespace(options.StateCookie.Name, StateCookieNameParameterName);

        var cookieOptions = options.StateCookie.Build(httpContext, _systemClock.UtcNow);
        httpContext.Response.Cookies.Delete(options.StateCookie.Name, cookieOptions);
    }

    private static User ParseUser(BankIdUiResult uiResult) => new(uiResult.PersonalIdentityNumber, uiResult.Name, uiResult.GivenName, uiResult.Surname);
    private static Device ParseDevice(BankIdUiResult uiResult) => new(uiResult.DetectedIpAddress);
    private static Cert ParseCert(BankIdUiResult uiResult) => new Cert(uiResult.CertNotBefore, uiResult.CertNotAfter);

}
