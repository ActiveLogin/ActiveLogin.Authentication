using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Helpers;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.Core.Events;
using ActiveLogin.Authentication.BankId.Core.Flow;
using ActiveLogin.Identity.Swedish;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Sign;

public class BankIdSignService : IBankIdSignService
{
    private const string StateCookieNameParameterName = "StateCookie.Name";
    private readonly PathString _signInitPath = new($"/{BankIdConstants.Routes.ActiveLoginAreaName}/{BankIdConstants.Routes.BankIdPathName}/{BankIdConstants.Routes.BankIdSignControllerName}");

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IOptionsSnapshot<BankIdSignOptions> _optionsSnapshot;
    private readonly IBankIdFlowSystemClock _systemClock;
    private readonly IBankIdUiSignStateProtector _bankIdUiSignStateProtector;
    private readonly IBankIdUiOptionsProtector _uiOptionsProtector;

    public BankIdSignService(IHttpContextAccessor httpContextAccessor, IOptionsSnapshot<BankIdSignOptions> optionsSnapshot, IBankIdFlowSystemClock systemClock, IBankIdUiSignStateProtector bankIdUiSignStateProtector, IBankIdUiOptionsProtector uiOptionsProtector)
    {
        _httpContextAccessor = httpContextAccessor;
        _optionsSnapshot = optionsSnapshot;
        _systemClock = systemClock;
        _bankIdUiSignStateProtector = bankIdUiSignStateProtector;
        _uiOptionsProtector = uiOptionsProtector;
    }

    public IActionResult InitiateSign(BankIdSignProperties properties, PathString callbackPath, string configKey)
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

        //TODO
        //var detectedDevice = _bankIdSupportedDeviceDetector.Detect();
        //await _bankIdEventTrigger.TriggerAsync(new BankIdAspNetChallengeSuccessEvent(detectedDevice, uiOptions.ToBankIdFlowOptions()));

        var signUrl = GetUiInitUrl(httpContext, callbackPath, uiOptions);
        return new RedirectResult(signUrl);
    }

    public Task<BankIdSignResult?> GetSignResultAsync()
    {
        throw new NotImplementedException();

        //var httpContext = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("Can't access HttpContext");
        //var detectedDevice = _bankIdSupportedDeviceDetector.Detect();

        //var state = GetStateFromCookie(httpContext, _);
        //if (state == null)
        //{
        //    return await HandleRemoteAuthenticateFail(BankIdConstants.ErrorMessages.InvalidStateCookie, detectedDevice);
        //}

        //DeleteStateCookie();

        //var protectedUiResult = Request.Query[BankIdConstants.QueryStringParameters.UiResult];
        //if (string.IsNullOrEmpty(protectedUiResult))
        //{
        //    return await HandleRemoteAuthenticateFail(BankIdConstants.ErrorMessages.InvalidUiResult, detectedDevice);
        //}

        //var uiResult = _uiResultProtector.Unprotect(protectedUiResult);
        //if (!uiResult.IsSuccessful)
        //{
        //    return await HandleRemoteAuthenticateFail(BankIdConstants.ErrorMessages.InvalidUiResult, detectedDevice);
        //}

        //var properties = state.AuthenticationProperties;
        //var ticket = await GetAuthenticationTicket(uiResult, properties);

        ////await _bankIdEventTrigger.TriggerAsync(new BankIdAspNetAuthenticateSuccessEvent(
        ////    PersonalIdentityNumber.Parse(uiResult.PersonalIdentityNumber),
        ////    detectedDevice
        ////));

        //return HandleRequestResult.Success(ticket);
    }

    private void AppendStateCookie(HttpContext httpContext, BankIdSignProperties properties, BankIdSignOptions options, string configKey)
    {
        Validators.ThrowIfNullOrWhitespace(options.StateCookie.Name, StateCookieNameParameterName);

        var state = new BankIdUiSignState(configKey, properties);
        var cookieOptions = options.StateCookie.Build(httpContext, _systemClock.UtcNow);
        var cookieValue = _bankIdUiSignStateProtector.Protect(state);

        httpContext.Response.Cookies.Append(options.StateCookie.Name, cookieValue, cookieOptions);
    }

    private string GetUiInitUrl(HttpContext httpContext, PathString callbackPath, BankIdUiOptions uiOptions)
    {
        var pathBase = httpContext.Request.PathBase;
        var signUrl = pathBase.Add(_signInitPath);
        var returnUrl = pathBase.Add(callbackPath);
        var protectedUiOptions = _uiOptionsProtector.Protect(uiOptions);

        var queryBuilder = new QueryBuilder(new Dictionary<string, string>
        {
            { BankIdConstants.QueryStringParameters.ReturnUrl, returnUrl },
            { BankIdConstants.QueryStringParameters.UiOptions, protectedUiOptions }
        });

        return $"{signUrl}{queryBuilder.ToQueryString()}";
    }

    private BankIdUiSignState? GetStateFromCookie(HttpContext httpContext, string stateCookieName)
    {
        Validators.ThrowIfNullOrWhitespace(stateCookieName, StateCookieNameParameterName);

        var protectedState = httpContext.Request.Cookies[stateCookieName];
        if (string.IsNullOrEmpty(protectedState))
        {
            return null;
        }

        return _bankIdUiSignStateProtector.Unprotect(protectedState);
    }

    private void DeleteStateCookie(HttpContext httpContext, CookieBuilder cookieBuilder, string stateCookieName, DateTimeOffset utcNow)
    {
        Validators.ThrowIfNullOrWhitespace(stateCookieName, StateCookieNameParameterName);

        var cookieOptions = cookieBuilder.Build(httpContext, utcNow);
        httpContext.Response.Cookies.Delete(stateCookieName, cookieOptions);
    }
}
