using System.Text.Json;

using ActiveLogin.Authentication.BankId.Api.UserMessage;
using ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Helpers;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.Core.StateHandling;
using ActiveLogin.Authentication.BankId.Core.UserMessage;

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Controllers;

[NonController]
public abstract class BankIdUiControllerBase : Controller
{
    private readonly IAntiforgery _antiforgery;
    private readonly IBankIdUserMessageLocalizer _bankIdUserMessageLocalizer;
    private readonly IBankIdUiOptionsProtector _uiOptionsProtector;
    private readonly IStringLocalizer<BankIdHandler> _localizer;
    private readonly IBankIdInvalidStateHandler _bankIdInvalidStateHandler;

    protected BankIdUiControllerBase(
        IAntiforgery antiforgery,
        IBankIdUserMessageLocalizer bankIdUserMessageLocalizer,
        IBankIdUiOptionsProtector uiOptionsProtector,
        IStringLocalizer<BankIdHandler> localizer,
        IBankIdInvalidStateHandler bankIdInvalidStateHandler)
    {
        _antiforgery = antiforgery;
        _bankIdUserMessageLocalizer = bankIdUserMessageLocalizer;
        _uiOptionsProtector = uiOptionsProtector;
        _localizer = localizer;
        _bankIdInvalidStateHandler = bankIdInvalidStateHandler;
    }

    protected async Task<ActionResult> Initialize(string returnUrl, string protectedUiOptions, string viewName)
    {
        Validators.ThrowIfNullOrWhitespace(returnUrl);
        Validators.ThrowIfNullOrWhitespace(protectedUiOptions, BankIdConstants.QueryStringParameters.UiOptions);

        if (!Url.IsLocalUrl(returnUrl))
        {
            throw new ArgumentException(BankIdConstants.ErrorMessages.InvalidReturnUrl);
        }

        var uiOptions = _uiOptionsProtector.Unprotect(protectedUiOptions);
        if (!HasStateCookie(uiOptions))
        {
            var invalidStateContext = new BankIdInvalidStateContext(uiOptions.CancelReturnUrl);
            await _bankIdInvalidStateHandler.HandleAsync(invalidStateContext);

            return new EmptyResult();
        }

        protectedUiOptions = _uiOptionsProtector.Protect(uiOptions);
        var antiforgeryTokens = _antiforgery.GetAndStoreTokens(HttpContext);
        var viewModel = GetUiViewModel(returnUrl, protectedUiOptions, uiOptions, antiforgeryTokens);

        return View(viewName, viewModel);
    }

    private bool HasStateCookie(BankIdUiOptions uiOptions)
    {
        if (string.IsNullOrEmpty(uiOptions.StateCookieName)
            || !HttpContext.Request.Cookies.ContainsKey(uiOptions.StateCookieName))
        {
            return false;
        }

        return !string.IsNullOrEmpty(HttpContext.Request.Cookies[uiOptions.StateCookieName]);
    }

    private BankIdUiViewModel GetUiViewModel(string returnUrl, string uiOptions, BankIdUiOptions unprotectedUiOptions, AntiforgeryTokenSet antiforgeryTokens)
    {
        Validators.ThrowIfNullOrWhitespace(antiforgeryTokens.RequestToken, nameof(antiforgeryTokens.RequestToken));

        var initialStatusMessage = GetInitialStatusMessage(unprotectedUiOptions);
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
            Url.Content(unprotectedUiOptions.CancelReturnUrl),
            uiOptions,
            unprotectedUiOptions,
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

    private static MessageShortName GetInitialStatusMessage(BankIdUiOptions uiOptions)
    {
        return uiOptions.SameDevice
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
