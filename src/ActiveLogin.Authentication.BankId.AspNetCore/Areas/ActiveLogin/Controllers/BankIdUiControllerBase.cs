using ActiveLogin.Authentication.BankId.Api.UserMessage;
using ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Cookies;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Helpers;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Payment;
using ActiveLogin.Authentication.BankId.AspNetCore.Sign;
using ActiveLogin.Authentication.BankId.AspNetCore.StateHandling;
using ActiveLogin.Authentication.BankId.Core;
using ActiveLogin.Authentication.BankId.Core.UserMessage;

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Controllers;

[NonController]
public abstract class BankIdUiControllerBase<T>(
    IAntiforgery antiforgery,
    IStringLocalizer<ActiveLoginResources> localizer,
    IBankIdUserMessageLocalizer bankIdUserMessageLocalizer,
    IBankIdInvalidStateHandler bankIdInvalidStateHandler,
    IBankIdUiOptionsCookieManager uiOptionsCookieManager,
    IStateStorage stateStorage
) : Controller
    where T : BankIdUiState
{
    private readonly IAntiforgery _antiforgery = antiforgery;
    private readonly IStringLocalizer<ActiveLoginResources> _localizer = localizer;
    private readonly IBankIdUserMessageLocalizer _bankIdUserMessageLocalizer = bankIdUserMessageLocalizer;
    private readonly IBankIdInvalidStateHandler _bankIdInvalidStateHandler = bankIdInvalidStateHandler;
    private readonly IBankIdUiOptionsCookieManager _uiOptionsCookieManager = uiOptionsCookieManager;
    private readonly IStateStorage stateStorage = stateStorage;

    protected Task<T?> GetUIState(BankIdUiOptions uiOptions)
    {
        var cookie = HttpContext.Request.Cookies[uiOptions.StateKeyCookieName];
        if (cookie is null)
        {
            return Task.FromResult<T?>(default);
        }
        var stateKey = new StateKey(cookie);
        return stateStorage.GetAsync<T>(stateKey);
    }

    protected async Task<ActionResult> Initialize(string returnUrl, string apiControllerName, string uiOptionsGuid, string viewName)
    {
        Validators.ThrowIfNullOrWhitespace(returnUrl);
        Validators.ThrowIfNullOrWhitespace(uiOptionsGuid, BankIdConstants.QueryStringParameters.UiOptions);

        if (!Url.IsLocalUrl(returnUrl))
        {
            throw new ArgumentException(BankIdConstants.ErrorMessages.InvalidReturnUrl);
        }

        var uiOptions = _uiOptionsCookieManager.Retrieve(uiOptionsGuid) ?? throw new InvalidOperationException(BankIdConstants.ErrorMessages.InvalidUiOptions);
        if (!HasStateCookie(uiOptions))
        {
            var invalidStateContext = new BankIdInvalidStateContext(uiOptions.CancelReturnUrl);
            await _bankIdInvalidStateHandler.HandleAsync(invalidStateContext);

            return new EmptyResult();
        }

        var state = await GetUIState(uiOptions);

        if (state == null)
        {
            var invalidStateContext = new BankIdInvalidStateContext(uiOptions.CancelReturnUrl);
            await _bankIdInvalidStateHandler.HandleAsync(invalidStateContext);

            return new EmptyResult();
        }

        var antiforgeryTokens = _antiforgery.GetAndStoreTokens(HttpContext);
        var viewModel = GetUiViewModel(returnUrl, apiControllerName, uiOptionsGuid, uiOptions, state, antiforgeryTokens);

        return View(viewName, viewModel);
    }

    private bool HasStateCookie(BankIdUiOptions uiOptions)
    {
        if (string.IsNullOrEmpty(uiOptions.StateKeyCookieName))
        {
            return false;
        }

        if (!HttpContext.Request.Cookies.ContainsKey(uiOptions.StateKeyCookieName))
        {
            return false;
        }

        if (string.IsNullOrEmpty(HttpContext.Request.Cookies[uiOptions.StateKeyCookieName]))
        {
            return false;
        }

        return true;
    }

    private BankIdUiViewModel GetUiViewModel(string returnUrl, string apiControllerName, string uiOptionsGuid, BankIdUiOptions unprotectedUiOptions, BankIdUiState uiState, AntiforgeryTokenSet antiforgeryTokens)
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

            UiOptionsGuid = uiOptionsGuid
        };

        var localizedStartAppButtonText = _localizer["StartApp_Button"];
        var localizedCancelButtonText = _localizer["Cancel_Button"];
        var localizedQrCodeImageAltText = _localizer["Qr_Code_Image"];

        if (uiState is BankIdUiSignState signState)
        {
            var uiSignData = new BankIdUiSignData
            {
                UserVisibleData = signState.BankIdSignProperties.UserVisibleData,
                UserVisibleDataFormat = signState.BankIdSignProperties.UserVisibleDataFormat
            };
            return new BankIdUiViewModel(uiScriptConfiguration, uiScriptInitState, signData:uiSignData)
            {
                LocalizedPageHeader = _localizer["Sign_Header"],
                LocalizedPageTitle = _localizer["Sign_Title"],

                LocalizedStartAppButtonText = localizedStartAppButtonText,
                LocalizedCancelButtonText = localizedCancelButtonText,
                LocalizedQrCodeImageAltText = localizedQrCodeImageAltText
            };
        }

        if (uiState is BankIdUiPaymentState paymentState)
        {
            var uiPaymentData = new BankIdUiPaymentData
            {
                UserVisibleData = paymentState.BankIdPaymentProperties.UserVisibleData,
                UserVisibleDataFormat = paymentState.BankIdPaymentProperties.UserVisibleDataFormat
            };
            return new BankIdUiViewModel(uiScriptConfiguration, uiScriptInitState, paymentData: uiPaymentData)
            {
                LocalizedPageHeader = _localizer["Payment_Header"],
                LocalizedPageTitle = _localizer["Payment_Title"],

                LocalizedStartAppButtonText = localizedStartAppButtonText,
                LocalizedCancelButtonText = localizedCancelButtonText,
                LocalizedQrCodeImageAltText = localizedQrCodeImageAltText
            };
        }

        return new BankIdUiViewModel(uiScriptConfiguration, uiScriptInitState)
        {
            LocalizedPageHeader = _localizer["Auth_Header"],
            LocalizedPageTitle = _localizer["Auth_Title"],

            LocalizedStartAppButtonText = localizedStartAppButtonText,
            LocalizedCancelButtonText = localizedCancelButtonText,
            LocalizedQrCodeImageAltText = localizedQrCodeImageAltText
        };
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
            : MessageShortName.RFA1;
    }
}
