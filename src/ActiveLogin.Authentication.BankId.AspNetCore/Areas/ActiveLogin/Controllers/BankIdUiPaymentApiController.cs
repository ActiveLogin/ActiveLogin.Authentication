using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.Api.UserMessage;
using ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Helpers;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Payment;
using ActiveLogin.Authentication.BankId.Core.Flow;
using ActiveLogin.Authentication.BankId.Core.Models;
using ActiveLogin.Authentication.BankId.Core.UserMessage;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Controllers;

[Area(BankIdConstants.Routes.ActiveLoginAreaName)]
[Route($"/[area]/{BankIdConstants.Routes.BankIdPathName}/{BankIdConstants.Routes.BankIdPaymentControllerPath}/{BankIdConstants.Routes.BankIdApiControllerPath}/")]
[ApiController]
[AllowAnonymous]
[NonController]
public class BankIdUiPaymentApiController : BankIdUiApiControllerBase
{
    private readonly IBankIdUiStateProtector _bankIdUiStateProtector;

    public BankIdUiPaymentApiController(
        IBankIdFlowService bankIdFlowService,
        IBankIdUiOrderRefProtector orderRefProtector,
        IBankIdQrStartStateProtector qrStartStateProtector,
        IBankIdUiOptionsProtector uiOptionsProtector,
        IBankIdUserMessage bankIdUserMessage,
        IBankIdUserMessageLocalizer bankIdUserMessageLocalizer,
        IBankIdUiResultProtector uiAuthResultProtector,
        IBankIdUiStateProtector bankIdUiStateProtector)
        : base(bankIdFlowService, orderRefProtector, qrStartStateProtector, uiOptionsProtector, bankIdUserMessage, bankIdUserMessageLocalizer, uiAuthResultProtector)
    {
        _bankIdUiStateProtector = bankIdUiStateProtector;
    }

    [ValidateAntiForgeryToken]
    [HttpPost(BankIdConstants.Routes.BankIdApiInitializeActionName)]
    public async Task<ActionResult<BankIdUiApiInitializeResponse>> Initialize(BankIdUiApiInitializeRequest request)
    {
        Validators.ThrowIfNullOrWhitespace(request.ReturnUrl, nameof(request.ReturnUrl));
        Validators.ThrowIfNullOrWhitespace(request.UiOptions, nameof(request.UiOptions));

        var uiOptions = UiOptionsProtector.Unprotect(request.UiOptions);

        var state = GetStateFromCookie(uiOptions);
        if(state == null)
        {
            throw new InvalidOperationException(BankIdConstants.ErrorMessages.InvalidStateCookie);
        }

        BankIdFlowInitializeResult bankIdFlowInitializeResult;
        try
        {
            var returnRedirectUrl = Url.Action(BankIdConstants.Routes.BankIdPaymentInitActionName, BankIdConstants.Routes.BankIdPaymentControllerName, new
            {
                returnUrl = request.ReturnUrl,
                uiOptions = request.UiOptions
            }, protocol: Request.Scheme) ?? throw new Exception(BankIdConstants.ErrorMessages.CouldNotGetUrlFor(BankIdConstants.Routes.BankIdPaymentControllerName, BankIdConstants.Routes.BankIdPaymentInitActionName));

            bankIdFlowInitializeResult = await BankIdFlowService.InitializePayment(
                uiOptions.ToBankIdFlowOptions(),
                new BankIdPaymentData(state.BankIdPaymentProperties.TransactionType, state.BankIdPaymentProperties.RecipientName)
                {
                    Money = state.BankIdPaymentProperties.Money,
                    RiskWarning = state.BankIdPaymentProperties.RiskWarning,
                    RiskFlags = state.BankIdPaymentProperties.RiskFlags,
                    Items = state.BankIdPaymentProperties.Items,
                    UserVisibleData = state.BankIdPaymentProperties.UserVisibleData,
                    UserNonVisibleData = state.BankIdPaymentProperties.UserNonVisibleData,
                    UserVisibleDataFormat = state.BankIdPaymentProperties.UserVisibleDataFormat,
                    RequiredPersonalIdentityNumber = state.BankIdPaymentProperties.RequiredPersonalIdentityNumber,
                    RequireMrtd = state.BankIdPaymentProperties.RequireMrtd,
                    RequirePinCode = state.BankIdPaymentProperties.RequirePinCode,
                    CertificatePolicies = state.BankIdPaymentProperties.BankIdCertificatePolicies,
                    CardReader = state.BankIdPaymentProperties.CardReader,
                },
                returnRedirectUrl);
        }
        catch (BankIdApiException bankIdApiException)
        {
            var errorStatusMessage = GetBankIdApiExceptionStatusMessage(bankIdApiException);
            return BadRequestJsonResult(new BankIdUiApiErrorResponse(errorStatusMessage));
        }

        var protectedOrderRef = OrderRefProtector.Protect(new BankIdUiOrderRef(bankIdFlowInitializeResult.BankIdResponse.OrderRef));
        switch (bankIdFlowInitializeResult.LaunchType)
        {
            case BankIdFlowInitializeLaunchTypeOtherDevice otherDevice:
            {
                var protectedQrStartState = QrStartStateProtector.Protect(otherDevice.QrStartState);
                return OkJsonResult(BankIdUiApiInitializeResponse.ManualLaunch(protectedOrderRef, protectedQrStartState, otherDevice.QrCodeBase64Encoded));
            }
            case BankIdFlowInitializeLaunchTypeSameDevice sameDevice when sameDevice.BankIdLaunchInfo.DeviceWillReloadPageOnReturnFromBankIdApp:
            {
                return OkJsonResult(BankIdUiApiInitializeResponse.AutoLaunch(protectedOrderRef, sameDevice.BankIdLaunchInfo.LaunchUrl, sameDevice.BankIdLaunchInfo.DeviceMightRequireUserInteractionToLaunchBankIdApp));
            }
            case BankIdFlowInitializeLaunchTypeSameDevice sameDevice:
            {
                return OkJsonResult(BankIdUiApiInitializeResponse.AutoLaunchAndCheckStatus(protectedOrderRef, sameDevice.BankIdLaunchInfo.LaunchUrl, sameDevice.BankIdLaunchInfo.DeviceMightRequireUserInteractionToLaunchBankIdApp));
            }
            default:
            {
                throw new InvalidOperationException(BankIdConstants.ErrorMessages.UnknownFlowLaunchType);
            }
        }
    }

    private BankIdUiPaymentState? GetStateFromCookie(BankIdUiOptions uiOptions)
    {
        var protectedState = Request.Cookies[uiOptions.StateCookieName];
        if (protectedState == null)
        {
            throw new InvalidOperationException(BankIdConstants.ErrorMessages.InvalidStateCookie);
        }

        return _bankIdUiStateProtector.Unprotect(protectedState) as BankIdUiPaymentState;
    }
}
