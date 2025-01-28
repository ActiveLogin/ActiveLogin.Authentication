using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Auth;
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
    IBankIdUiOptionsProtector uiOptionsProtector,
    IBankIdSupportedDeviceDetector bankIdSupportedDeviceDetector,
    IBankIdEventTrigger bankIdEventTrigger,
    IBankIdUiResultProtector uiResultProtector
) : IBankIdSignService
{
    private const string StateCookieNameParameterName = "StateCookie.Name";
    private readonly PathString _signInitPath = new($"/{BankIdConstants.Routes.ActiveLoginAreaName}/{BankIdConstants.Routes.BankIdPathName}/{BankIdConstants.Routes.BankIdSignControllerPath}");

    public async Task InitiateSignAsync(BankIdSignProperties properties, string callbackPath, string configKey)
    {
        var httpContext = httpContextAccessor.HttpContext ?? throw new InvalidOperationException(BankIdConstants.ErrorMessages.CouldNotAccessHttpContext);
        var options = optionsSnapshot.Get(configKey);

        var stateKey = await SaveState(properties, configKey);
        AppendCookie(httpContext, stateKey, options);

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
    }

    private void AppendCookie(HttpContext httpContext, StateKey key, BankIdSignOptions options)
    {
        Validators.ThrowIfNullOrWhitespace(options.StateCookie.Name, StateCookieNameParameterName);

        var cookieOptions = options.StateCookie.Build(httpContext, systemClock.UtcNow);
        httpContext.Response.Cookies.Append(options.StateCookie.Name, key, cookieOptions);
    }

    public async Task<BankIdSignResult?> GetSignResultAsync(string configKey)
    {
        Validators.ThrowIfNullOrWhitespace(configKey, nameof(configKey));
        var detectedDevice = bankIdSupportedDeviceDetector.Detect();

        var options = optionsSnapshot.Get(configKey);
        Validators.ThrowIfNullOrWhitespace(options.StateCookie.Name, StateCookieNameParameterName);

        var httpContext = httpContextAccessor.HttpContext ?? throw new InvalidOperationException(BankIdConstants.ErrorMessages.CouldNotAccessHttpContext);
        var signState = await GetState(httpContext, options.StateCookie.Name);

        if (signState == null)
        {
            await bankIdEventTrigger.TriggerAsync(new BankIdSignFailureEvent(BankIdConstants.ErrorMessages.InvalidStateCookie, detectedDevice));
            throw new ArgumentException(BankIdConstants.ErrorMessages.StateNotFound);
        }

        if (!signState.ConfigKey.Equals(configKey))
        {
            throw new ArgumentException(BankIdConstants.ErrorMessages.InvalidState);
        }

        DeleteCookie(httpContext, options);

        if (!httpContext.Request.HasFormContentType)
        {
            await bankIdEventTrigger.TriggerAsync(new BankIdSignFailureEvent(BankIdConstants.ErrorMessages.InvalidUiResult, detectedDevice));
            throw new ArgumentException(BankIdConstants.ErrorMessages.InvalidUiResult);
        }

        await antiforgery.ValidateRequestAsync(httpContext);

        var protectedUiResult = httpContext.Request.Form[BankIdConstants.FormParameters.UiResult];
        if (StringValues.IsNullOrEmpty(protectedUiResult))
        {
            await bankIdEventTrigger.TriggerAsync(new BankIdSignFailureEvent(BankIdConstants.ErrorMessages.InvalidUiResult, detectedDevice));
            return new BankIdSignResult(false, signState.BankIdSignProperties);
        }

        var uiResult = uiResultProtector.Unprotect(protectedUiResult.ToString());
        if (!uiResult.IsSuccessful)
        {
            await bankIdEventTrigger.TriggerAsync(new BankIdSignFailureEvent(BankIdConstants.ErrorMessages.InvalidUiResult, detectedDevice));
            return new BankIdSignResult(false, signState.BankIdSignProperties);
        }

        await bankIdEventTrigger.TriggerAsync(new BankIdSignSuccessEvent(
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

        var protectedUiOptions = uiOptionsProtector.Protect(uiOptions);

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
        return bankIdStateStorage.RemoveAsync<BankIdUiSignState>(stateKey);
    }

    private Task<StateKey> SaveState(BankIdSignProperties properties, string configKey)
    {
        var state = new BankIdUiSignState(configKey, properties);
        return bankIdStateStorage.WriteAsync(state);
    }

    private void DeleteCookie(HttpContext httpContext, BankIdSignOptions options)
    {
        Validators.ThrowIfNullOrWhitespace(options.StateCookie.Name, StateCookieNameParameterName);

        var cookieOptions = options.StateCookie.Build(httpContext, systemClock.UtcNow);
        httpContext.Response.Cookies.Delete(options.StateCookie.Name, cookieOptions);
    }
}
