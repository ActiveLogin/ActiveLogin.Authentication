using ActiveLogin.Authentication.BankId.Api.Models;
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

namespace ActiveLogin.Authentication.BankId.AspNetCore.Payment;

public class BankIdPaymentService(
    IHttpContextAccessor httpContextAccessor,
    IAntiforgery antiforgery,
    IOptionsSnapshot<BankIdPaymentOptions> optionsSnapshot,
    IBankIdFlowSystemClock systemClock,
    IStateStorage bankIdStateStorage,
    IBankIdDataStateProtector<BankIdUiOptions> uiOptionsProtector,
    IBankIdSupportedDeviceDetector bankIdSupportedDeviceDetector,
    IBankIdEventTrigger bankIdEventTrigger,
    IBankIdDataStateProtector<BankIdUiResult> uiResultProtector
) : IBankIdPaymentService
{
    private const string StateCookieNameParameterName = "StateCookie.Name";
    private readonly PathString _paymentInitPath = new($"/{BankIdConstants.Routes.ActiveLoginAreaName}/{BankIdConstants.Routes.BankIdPathName}/{BankIdConstants.Routes.BankIdPaymentControllerPath}");

    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IAntiforgery _antiforgery = antiforgery;

    private readonly IOptionsSnapshot<BankIdPaymentOptions> _optionsSnapshot = optionsSnapshot;
    private readonly IBankIdFlowSystemClock _systemClock = systemClock;
    private readonly IStateStorage _bankIdStateStorage = bankIdStateStorage;
    private readonly IBankIdDataStateProtector<BankIdUiResult> _uiResultProtector = uiResultProtector;
    private readonly IBankIdDataStateProtector<BankIdUiOptions> _uiOptionsProtector = uiOptionsProtector;
    private readonly IBankIdSupportedDeviceDetector _bankIdSupportedDeviceDetector = bankIdSupportedDeviceDetector;
    private readonly IBankIdEventTrigger _bankIdEventTrigger = bankIdEventTrigger;

    public async Task InitiatePaymentAsync(BankIdPaymentProperties properties, string callbackPath, string configKey)
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

        var paymentUrl = GetUiInitUrl(httpContext, callbackPath, uiOptions);

        httpContext.Response.Redirect(paymentUrl);
    }

    private void AppendCookie(HttpContext httpContext, StateKey key, BankIdPaymentOptions options)
    {
        Validators.ThrowIfNullOrWhitespace(options.StateCookie.Name, StateCookieNameParameterName);

        var cookieOptions = options.StateCookie.Build(httpContext, _systemClock.UtcNow);
        httpContext.Response.Cookies.Append(options.StateCookie.Name, key, cookieOptions);
    }

    public async Task<BankIdPaymentResult?> GetPaymentResultAsync(string configKey)
    {
        Validators.ThrowIfNullOrWhitespace(configKey, nameof(configKey));
        var options = _optionsSnapshot.Get(configKey);

        var detectedDevice = _bankIdSupportedDeviceDetector.Detect();
        Validators.ThrowIfNullOrWhitespace(options.StateCookie.Name, StateCookieNameParameterName);

        var httpContext = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException(BankIdConstants.ErrorMessages.CouldNotAccessHttpContext);
        var state = await GetState(httpContext, options.StateCookie.Name);

        if (state == null)
        {
            await _bankIdEventTrigger.TriggerAsync(new BankIdPaymentFailureEvent(BankIdConstants.ErrorMessages.InvalidStateCookie, detectedDevice));
            throw new ArgumentException(BankIdConstants.ErrorMessages.InvalidStateCookie);
        }

        if (!state.ConfigKey.Equals(configKey))
        {
            throw new ArgumentException(BankIdConstants.ErrorMessages.InvalidStateCookie);
        }

        DeleteStateCookie(httpContext, options);

        if (!httpContext.Request.HasFormContentType)
        {
            await _bankIdEventTrigger.TriggerAsync(new BankIdPaymentFailureEvent(BankIdConstants.ErrorMessages.InvalidUiResult, detectedDevice));
            throw new ArgumentException(BankIdConstants.ErrorMessages.InvalidUiResult);
        }

        await _antiforgery.ValidateRequestAsync(httpContext);

        var protectedUiResult = httpContext.Request.Form[BankIdConstants.FormParameters.UiResult];
        if (StringValues.IsNullOrEmpty(protectedUiResult))
        {
            await _bankIdEventTrigger.TriggerAsync(new BankIdPaymentFailureEvent(BankIdConstants.ErrorMessages.InvalidUiResult, detectedDevice));
            return new BankIdPaymentResult(false, state.BankIdPaymentProperties);
        }

        var uiResult = _uiResultProtector.Unprotect(protectedUiResult.ToString());
        if (!uiResult.IsSuccessful)
        {
            await _bankIdEventTrigger.TriggerAsync(new BankIdPaymentFailureEvent(BankIdConstants.ErrorMessages.InvalidUiResult, detectedDevice));
            return new BankIdPaymentResult(false, state.BankIdPaymentProperties);
        }

        await _bankIdEventTrigger.TriggerAsync(new BankIdPaymentSuccessEvent(
            PersonalIdentityNumber.Parse(uiResult.PersonalIdentityNumber),
            detectedDevice
        ));

        return BankIdPaymentResult.Success(
            state.BankIdPaymentProperties,
            uiResult.GetCompletionData()
        );
    }

    private async Task<BankIdUiPaymentState?> GetState(HttpContext httpContext, string stateCookieName)
    {
        var cookie = httpContext.Request.Cookies[stateCookieName];
        if (cookie is null)
        {
            return null;
        }

        var stateKey = new StateKey(cookie);
        return await _bankIdStateStorage.GetAsync<BankIdUiPaymentState>(stateKey);
    }

    private Task<StateKey> SaveState(BankIdPaymentProperties properties, string configKey)
    {
        var state = new BankIdUiPaymentState(configKey, properties);
        return _bankIdStateStorage.SetAsync(state);
    }

    private string GetUiInitUrl(HttpContext httpContext, string callbackPath, BankIdUiOptions uiOptions)
    {
        var pathBase = httpContext.Request.PathBase;
        var paymentUrl = pathBase.Add(_paymentInitPath);

        var protectedUiOptions = _uiOptionsProtector.Protect(uiOptions);

        var queryBuilder = QueryStringGenerator.ToQueryString(
            new Dictionary<string, string>
            {
                { BankIdConstants.QueryStringParameters.ReturnUrl, callbackPath },
                { BankIdConstants.QueryStringParameters.UiOptions, protectedUiOptions }
            });

        return $"{paymentUrl}{queryBuilder}";
    }

    private void DeleteStateCookie(HttpContext httpContext, BankIdPaymentOptions options)
    {
        Validators.ThrowIfNullOrWhitespace(options.StateCookie.Name, StateCookieNameParameterName);

        var cookieOptions = options.StateCookie.Build(httpContext, _systemClock.UtcNow);
        httpContext.Response.Cookies.Delete(options.StateCookie.Name, cookieOptions);
    }
}
