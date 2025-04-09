using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Helpers;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.Core;
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

public class BankIdSignService(
    IHttpContextAccessor httpContextAccessor,
    IAntiforgery antiforgery,
    IOptionsSnapshot<BankIdSignOptions> optionsSnapshot,
    IBankIdFlowSystemClock systemClock,
    IStateStorage bankIdStateStorage,
    IBankIdDataStateProtector<BankIdUiOptions> uiOptionsProtector,
    IBankIdSupportedDeviceDetector bankIdSupportedDeviceDetector,
    IBankIdEventTrigger bankIdEventTrigger,
    IBankIdDataStateProtector<BankIdUiResult> uiResultProtector
) : IBankIdSignService
{
    private const string StateCookieNameParameterName = "StateCookie.Name";
    private readonly PathString _signInitPath = new($"/{BankIdConstants.Routes.ActiveLoginAreaName}/{BankIdConstants.Routes.BankIdPathName}/{BankIdConstants.Routes.BankIdSignControllerPath}");

    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IAntiforgery _antiforgery = antiforgery;

    private readonly IOptionsSnapshot<BankIdSignOptions> _optionsSnapshot = optionsSnapshot;
    private readonly IBankIdFlowSystemClock _systemClock = systemClock;
    private readonly IStateStorage _bankIdStateStorage = bankIdStateStorage;
    private readonly IBankIdDataStateProtector<BankIdUiResult> _uiResultProtector = uiResultProtector;
    private readonly IBankIdDataStateProtector<BankIdUiOptions> _uiOptionsProtector = uiOptionsProtector;
    private readonly IBankIdSupportedDeviceDetector _bankIdSupportedDeviceDetector = bankIdSupportedDeviceDetector;
    private readonly IBankIdEventTrigger _bankIdEventTrigger = bankIdEventTrigger;

    public async Task InitiateSignAsync(BankIdSignProperties properties, string callbackPath, string configKey)
    {
        var httpContext = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException(BankIdConstants.ErrorMessages.CouldNotAccessHttpContext);
        var options = _optionsSnapshot.Get(configKey);

        var stateKey = await SaveState(properties, configKey);
        AppendCookie(httpContext, stateKey, options);

        var uiOptions = new BankIdUiOptions(
            options.BankIdCertificatePolicies,
            options.BankIdSameDevice,
            options.BankIdRequirePinCode,
            options.BankIdRequireMrtd,
            options.BankIdReturnRisk,
            BankIdHandlerHelper.GetCancelReturnUrl(properties.Items),
            options.StateCookie.Name ?? string.Empty,
            options.CardReader
        );

        var signUrl = GetUiInitUrl(httpContext, callbackPath, uiOptions);

        httpContext.Response.Redirect(signUrl);
    }

    private void AppendCookie(HttpContext httpContext, StateKey key, BankIdSignOptions options)
    {
        Validators.ThrowIfNullOrWhitespace(options.StateCookie.Name, StateCookieNameParameterName);

        var cookieOptions = options.StateCookie.Build(httpContext, _systemClock.UtcNow);
        httpContext.Response.Cookies.Append(options.StateCookie.Name, key, cookieOptions);
    }

    public async Task<BankIdSignResult?> GetSignResultAsync(string configKey)
    {
        Validators.ThrowIfNullOrWhitespace(configKey, nameof(configKey));
        var options = _optionsSnapshot.Get(configKey);

        var detectedDevice = _bankIdSupportedDeviceDetector.Detect();
        Validators.ThrowIfNullOrWhitespace(options.StateCookie.Name, StateCookieNameParameterName);

        var httpContext = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException(BankIdConstants.ErrorMessages.CouldNotAccessHttpContext);
        var signState = await GetState(httpContext, options.StateCookie.Name);

        if (signState == null)
        {
            await _bankIdEventTrigger.TriggerAsync(new BankIdSignFailureEvent(BankIdConstants.ErrorMessages.InvalidStateCookie, detectedDevice));
            throw new ArgumentException(BankIdConstants.ErrorMessages.StateNotFound);
        }

        if (!signState.ConfigKey.Equals(configKey))
        {
            throw new ArgumentException(BankIdConstants.ErrorMessages.InvalidState);
        }

        DeleteCookie(httpContext, options);

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
            return new BankIdSignResult(false, signState.BankIdSignProperties);
        }

        var uiResult = _uiResultProtector.Unprotect(protectedUiResult.ToString());
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
            uiResult.GetCompletionData()
        );
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

    private Task<BankIdUiSignState?> GetState(HttpContext httpContext, string stateCookieName)
    {
        var cookie = httpContext.Request.Cookies[stateCookieName];
        if (cookie is null)
        {
            return Task.FromResult<BankIdUiSignState?>(null);
        }

        var stateKey = new StateKey(cookie);
        return _bankIdStateStorage.GetAsync<BankIdUiSignState>(stateKey);
    }

    private Task<StateKey> SaveState(BankIdSignProperties properties, string configKey)
    {
        var state = new BankIdUiSignState(configKey, properties);
        return _bankIdStateStorage.SetAsync(state);
    }

    private void DeleteCookie(HttpContext httpContext, BankIdSignOptions options)
    {
        Validators.ThrowIfNullOrWhitespace(options.StateCookie.Name, StateCookieNameParameterName);

        var cookieOptions = options.StateCookie.Build(httpContext, _systemClock.UtcNow);
        httpContext.Response.Cookies.Delete(options.StateCookie.Name, cookieOptions);
    }
}
