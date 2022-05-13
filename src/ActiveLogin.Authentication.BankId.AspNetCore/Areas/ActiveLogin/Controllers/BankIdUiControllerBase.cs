using ActiveLogin.Authentication.BankId.Api.UserMessage;
using ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Auth;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Helpers;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Sign;
using ActiveLogin.Authentication.BankId.AspNetCore.StateHandling;
using ActiveLogin.Authentication.BankId.Core.UserMessage;

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Controllers;

[NonController]
public abstract class BankIdUiControllerBase : Controller
{
    private readonly IAntiforgery _antiforgery;
    private readonly IStringLocalizer<BankIdAuthHandler> _localizer;
    private readonly IBankIdUserMessageLocalizer _bankIdUserMessageLocalizer;
    private readonly IBankIdUiOptionsProtector _uiOptionsProtector;
    private readonly IBankIdInvalidStateHandler _bankIdInvalidStateHandler;
    private readonly IBankIdUiStateProtector _bankIdUiStateProtector;

    protected BankIdUiControllerBase(
        IAntiforgery antiforgery,
        IStringLocalizer<BankIdAuthHandler> localizer,
        IBankIdUserMessageLocalizer bankIdUserMessageLocalizer,
        IBankIdUiOptionsProtector uiOptionsProtector,
        IBankIdInvalidStateHandler bankIdInvalidStateHandler,
        IBankIdUiStateProtector bankIdUiStateProtector)
    {
        _antiforgery = antiforgery;
        _localizer = localizer;
        _bankIdUserMessageLocalizer = bankIdUserMessageLocalizer;
        _uiOptionsProtector = uiOptionsProtector;
        _bankIdInvalidStateHandler = bankIdInvalidStateHandler;
        _bankIdUiStateProtector = bankIdUiStateProtector;
    }

    protected async Task<ActionResult> Initialize(string returnUrl, string apiControllerName, string protectedUiOptions, string viewName)
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

        var protectedState = Request.Cookies[uiOptions.StateCookieName];
        if(protectedState == null)
        {
            var invalidStateContext = new BankIdInvalidStateContext(uiOptions.CancelReturnUrl);
            await _bankIdInvalidStateHandler.HandleAsync(invalidStateContext);

            return new EmptyResult();
        }
        var state = _bankIdUiStateProtector.Unprotect(protectedState);

        var viewModel = GetUiViewModel(returnUrl, apiControllerName, protectedUiOptions, uiOptions, state, antiforgeryTokens);

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

    private BankIdUiViewModel GetUiViewModel(string returnUrl, string apiControllerName, string protectedUiOptions, BankIdUiOptions unprotectedUiOptions, BankIdUiState uiState, AntiforgeryTokenSet antiforgeryTokens)
    {
        Validators.ThrowIfNullOrWhitespace(antiforgeryTokens.RequestToken, nameof(antiforgeryTokens.RequestToken));

        var initialStatusMessage = GetInitialStatusMessage(unprotectedUiOptions);

        var uiScriptConfiguration = new BankIdUiScriptConfiguration()
        {
            BankIdInitializeApiUrl = GetBankIdApiActionUrl(apiControllerName, BankIdConstants.Routes.BankIdApiInitializeActionName),
            BankIdStatusApiUrl = GetBankIdApiActionUrl(apiControllerName, BankIdConstants.Routes.BankIdApiStatusActionName),
            BankIdQrCodeApiUrl = GetBankIdApiActionUrl(apiControllerName, BankIdConstants.Routes.BankIdApiQrCodeActionName),
            BankIdCancelApiUrl = GetBankIdApiActionUrl(apiControllerName, BankIdConstants.Routes.BankIdApiCancelActionName),

            StatusRefreshIntervalMs = (int)BankIdConstants.StatusRefreshInterval.TotalMilliseconds,
            QrCodeRefreshIntervalMs = (int)BankIdConstants.QrCodeRefreshInterval.TotalMilliseconds,

            InitialStatusMessage = _bankIdUserMessageLocalizer.GetLocalizedString(initialStatusMessage),
            UnknownErrorMessage = _bankIdUserMessageLocalizer.GetLocalizedString(MessageShortName.RFA22),

            UnsupportedBrowserErrorMessage = _localizer[BankIdConstants.LocalizationKeys.UnsupportedBrowserErrorMessage]
        };

        var uiScriptInitState = new BankIdUiScriptInitState()
        {
            AntiXsrfRequestToken = antiforgeryTokens.RequestToken,

            ReturnUrl = returnUrl,
            CancelReturnUrl = Url.Content(unprotectedUiOptions.CancelReturnUrl),

            ProtectedUiOptions = protectedUiOptions
        };

        if(uiState is BankIdUiSignState signState)
        {
            var uiSignData = new BankIdUiSignData
            {
                UserVisibleData = signState.BankIdSignProperties.UserVisibleData
            };
            return new BankIdUiViewModel(uiScriptConfiguration, uiScriptInitState, uiSignData);
        }

        return new BankIdUiViewModel(uiScriptConfiguration, uiScriptInitState);
    }

    private string GetBankIdApiActionUrl(string apiControllerName, string action)
    {
        return Url.Action(action, apiControllerName)
               ?? throw new Exception(BankIdConstants.ErrorMessages.CouldNotGetUrlFor(apiControllerName, action));
    }

    private static MessageShortName GetInitialStatusMessage(BankIdUiOptions uiOptions)
    {
        return uiOptions.SameDevice
            ? MessageShortName.RFA13
            : MessageShortName.RFA1QR;
    }
}
