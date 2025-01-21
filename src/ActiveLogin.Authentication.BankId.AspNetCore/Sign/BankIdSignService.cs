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
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

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

    public Task InitiateSignAsync(BankIdSignProperties properties, string callbackPath, string configKey)
    {
        var httpContext = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException(BankIdConstants.ErrorMessages.CouldNotAccessHttpContext);
        var options = _optionsSnapshot.Get(configKey);

        AppendStateCookie(httpContext, properties, options, configKey);

        var uiOptions = new BankIdUiOptions(
            options.BankIdCertificatePolicies,
            options.BankIdAllowedRiskLevel,
            options.BankIdSameDevice,
            options.BankIdRequirePinCode,
            options.BankIdRequireMrtd,
            options.BankIdReturnRisk,
            BankIdHandlerHelper.GetCancelReturnUrl(properties.Items),
            options.StateCookie.Name ?? string.Empty
        );

        var signUrl = GetUiInitUrl(httpContext, callbackPath, uiOptions);

        httpContext.Response.Redirect(signUrl);

        return Task.CompletedTask;
    }

    public async Task<BankIdSignResult?> GetSignResultAsync(string configKey)
    {
        Validators.ThrowIfNullOrWhitespace(configKey, nameof(configKey));

        var detectedDevice = _bankIdSupportedDeviceDetector.Detect();
        var options = _optionsSnapshot.Get(configKey);
        var httpContext = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException(BankIdConstants.ErrorMessages.CouldNotAccessHttpContext);

        var state = GetStateCookie(httpContext, options);
        if (state == null)
        {
            await _bankIdEventTrigger.TriggerAsync(new BankIdSignFailureEvent(BankIdConstants.ErrorMessages.InvalidStateCookie, detectedDevice));
            throw new ArgumentException(BankIdConstants.ErrorMessages.InvalidStateCookie);
        }

        if (!state.ConfigKey.Equals(configKey))
        {
            throw new ArgumentException(BankIdConstants.ErrorMessages.InvalidStateCookie);
        }

        DeleteStateCookie(httpContext, options);

        if (!httpContext.Request.HasFormContentType)
        {
            await _bankIdEventTrigger.TriggerAsync(new BankIdSignFailureEvent(BankIdConstants.ErrorMessages.InvalidUiResult, detectedDevice));
            throw new ArgumentException(BankIdConstants.ErrorMessages.InvalidUiResult);
        }

        await _antiforgery.ValidateRequestAsync(httpContext);

        var protectedUiResult = httpContext.Request.Form[BankIdConstants.FormParameters.UiResult];
        if (StringValues.IsNullOrEmpty(protectedUiResult))
        {
            await _bankIdEventTrigger.TriggerAsync(new BankIdSignFailureEvent(BankIdConstants.ErrorMessages.InvalidUiResult, detectedDevice));
            return new BankIdSignResult(false, state.BankIdSignProperties);
        }

        var uiResult = _uiResultProtector.Unprotect(protectedUiResult.ToString());
        if (!uiResult.IsSuccessful)
        {
            await _bankIdEventTrigger.TriggerAsync(new BankIdSignFailureEvent(BankIdConstants.ErrorMessages.InvalidUiResult, detectedDevice));
            return new BankIdSignResult(false, state.BankIdSignProperties);
        }

        await _bankIdEventTrigger.TriggerAsync(new BankIdSignSuccessEvent(
            PersonalIdentityNumber.Parse(uiResult.PersonalIdentityNumber),
            detectedDevice
        ));

        return BankIdSignResult.Success(
            state.BankIdSignProperties,
            uiResult.GetCompletionData()
        );
    }

    private BankIdUiSignState? GetStateCookie(HttpContext httpContext, BankIdSignOptions options)
    {
        Validators.ThrowIfNullOrWhitespace(options.StateCookie.Name, StateCookieNameParameterName);

        var protectedState = httpContext.Request.Cookies[options.StateCookie.Name];
        if (string.IsNullOrEmpty(protectedState))
        {
            return null;
        }
        return _bankIdUiStateProtector.Unprotect(protectedState) as BankIdUiSignState;
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
}
