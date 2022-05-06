using System.Text.Json;

using ActiveLogin.Authentication.BankId.Api.UserMessage;
using ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Helpers;
using ActiveLogin.Authentication.BankId.Core.Models;
using ActiveLogin.Authentication.BankId.Core.StateHandling;
using ActiveLogin.Authentication.BankId.Core.UserMessage;

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Controllers;

[NonController]
public abstract class BankIdControllerBase : Controller
{
    private readonly IAntiforgery _antiforgery;
    private readonly IBankIdUserMessageLocalizer _bankIdUserMessageLocalizer;
    private readonly IBankIdLoginOptionsProtector _loginOptionsProtector;
    private readonly IStringLocalizer<BankIdHandler> _localizer;
    private readonly IBankIdInvalidStateHandler _bankIdInvalidStateHandler;

    protected BankIdControllerBase(
        IAntiforgery antiforgery,
        IBankIdUserMessageLocalizer bankIdUserMessageLocalizer,
        IBankIdLoginOptionsProtector loginOptionsProtector,
        IStringLocalizer<BankIdHandler> localizer,
        IBankIdInvalidStateHandler bankIdInvalidStateHandler)
    {
        _antiforgery = antiforgery;
        _bankIdUserMessageLocalizer = bankIdUserMessageLocalizer;
        _loginOptionsProtector = loginOptionsProtector;
        _localizer = localizer;
        _bankIdInvalidStateHandler = bankIdInvalidStateHandler;
    }

    protected async Task<ActionResult> Initialize(string returnUrl, string protectedLoginOptions, string viewName)
    {
        Validators.ThrowIfNullOrWhitespace(returnUrl);
        Validators.ThrowIfNullOrWhitespace(protectedLoginOptions, BankIdConstants.QueryStringParameters.LoginOptions);

        if (!Url.IsLocalUrl(returnUrl))
        {
            throw new ArgumentException(BankIdConstants.ErrorMessages.InvalidReturnUrl);
        }

        var loginOptions = _loginOptionsProtector.Unprotect(protectedLoginOptions);
        if (!HasStateCookie(loginOptions))
        {
            var invalidStateContext = new BankIdInvalidStateContext(loginOptions.CancelReturnUrl);
            await _bankIdInvalidStateHandler.HandleAsync(invalidStateContext);

            return new EmptyResult();
        }

        protectedLoginOptions = _loginOptionsProtector.Protect(loginOptions);
        var antiforgeryTokens = _antiforgery.GetAndStoreTokens(HttpContext);
        var viewModel = GetLoginViewModel(returnUrl, protectedLoginOptions, loginOptions, antiforgeryTokens);

        return View(viewName, viewModel);
    }

    private bool HasStateCookie(BankIdLoginOptions loginOptions)
    {
        if (string.IsNullOrEmpty(loginOptions.StateCookieName)
            || !HttpContext.Request.Cookies.ContainsKey(loginOptions.StateCookieName))
        {
            return false;
        }

        return !string.IsNullOrEmpty(HttpContext.Request.Cookies[loginOptions.StateCookieName]);
    }

    private BankIdUiViewModel GetLoginViewModel(string returnUrl, string loginOptions, BankIdLoginOptions unprotectedLoginOptions, AntiforgeryTokenSet antiforgeryTokens)
    {
        Validators.ThrowIfNullOrWhitespace(antiforgeryTokens.RequestToken, nameof(antiforgeryTokens.RequestToken));

        var initialStatusMessage = GetInitialStatusMessage(unprotectedLoginOptions);
        var loginScriptOptions = new BankIdUiScriptOptions(
            GetBankIdApiActionUrl(BankIdConstants.Routes.BankIdApiInitializeActionName),
            GetBankIdApiActionUrl(BankIdConstants.Routes.BankIdApiStatusActionName),
            GetBankIdApiActionUrl(BankIdConstants.Routes.BankIdApiQrCodeActionName),
            GetBankIdApiActionUrl(BankIdConstants.Routes.BankIdApiCancelActionName)
        )
        {
            StatusRefreshIntervalMs = (int)BankIdConstants.StatusRefreshInterval.TotalMilliseconds,
            QrCodeRefreshIntervalMs = (int)BankIdConstants.QrCodeRefreshInterval.TotalMilliseconds,

            InitialStatusMessage = _bankIdUserMessageLocalizer.GetLocalizedString(initialStatusMessage),
            UnknownErrorMessage = _bankIdUserMessageLocalizer.GetLocalizedString(MessageShortName.RFA22),

            UnsupportedBrowserErrorMessage = _localizer[BankIdConstants.LocalizationKeys.UnsupportedBrowserErrorMessage]
        };

        return new BankIdUiViewModel(
            returnUrl,
            Url.Content(unprotectedLoginOptions.CancelReturnUrl),
            loginOptions,
            unprotectedLoginOptions,
            loginScriptOptions,
            SerializeToJson(loginScriptOptions),
            antiforgeryTokens.RequestToken
        );
    }

    private string GetBankIdApiActionUrl(string action)
    {
        return Url.Action(action, BankIdConstants.Routes.BankIdApiControllerName)
               ?? throw new Exception(BankIdConstants.ErrorMessages.CouldNotGetUrlFor(BankIdConstants.Routes.BankIdApiControllerName, action));
    }

    private static MessageShortName GetInitialStatusMessage(BankIdLoginOptions loginOptions)
    {
        return loginOptions.SameDevice
            ? MessageShortName.RFA13
            : MessageShortName.RFA1QR;
    }

    private static string SerializeToJson<T>(T value)
    {
        if (value == null)
        {
            return string.Empty;
        }
        
        return JsonSerializer.Serialize(value, value.GetType(), BankIdConstants.JsonSerializerOptions);
    }
}
