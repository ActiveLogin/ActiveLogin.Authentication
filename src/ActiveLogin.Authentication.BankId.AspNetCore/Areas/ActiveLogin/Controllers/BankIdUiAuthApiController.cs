using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.Api.UserMessage;
using ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Cookies;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Helpers;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.Core.Flow;
using ActiveLogin.Authentication.BankId.Core.UserMessage;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Controllers;

[Area(BankIdConstants.Routes.ActiveLoginAreaName)]
[Route($"/[area]/{BankIdConstants.Routes.BankIdPathName}/{BankIdConstants.Routes.BankIdAuthControllerPath}/{BankIdConstants.Routes.BankIdApiControllerPath}/")]
[ApiController]
[AllowAnonymous]
[NonController]
public class BankIdUiAuthApiController(
    IBankIdFlowService bankIdFlowService,
    IBankIdUiOrderRefProtector orderRefProtector,
    IBankIdQrStartStateProtector qrStartStateProtector,
    IBankIdUiOptionsProtector uiOptionsProtector,
    IBankIdUiOptionsCookieManager uiOptionsCookieManager,
    IBankIdUserMessage bankIdUserMessage,
    IBankIdUserMessageLocalizer bankIdUserMessageLocalizer,
    IBankIdUiResultProtector uiAuthResultProtector
) : BankIdUiApiControllerBase(bankIdFlowService, orderRefProtector, qrStartStateProtector, uiOptionsProtector, uiOptionsCookieManager, bankIdUserMessage, bankIdUserMessageLocalizer, uiAuthResultProtector)
{
    [ValidateAntiForgeryToken]
    [HttpPost(BankIdConstants.Routes.BankIdApiInitializeActionName)]
    public async Task<ActionResult<BankIdUiApiInitializeResponse>> Initialize(BankIdUiApiInitializeRequest request)
    {
        Validators.ThrowIfNullOrWhitespace(request.ReturnUrl, nameof(request.ReturnUrl));

        var uiOptions = ResolveProtectedUiOptions();

        BankIdFlowInitializeResult bankIdFlowInitializeResult;
        try
        {
            bankIdFlowInitializeResult = await BankIdFlowService.InitializeAuth(uiOptions.ToBankIdFlowOptions(), request.ReturnUrl);
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
            case BankIdFlowInitializeLaunchTypeSameDevice sameDevice:
            {
                return OkJsonResult(BankIdUiApiInitializeResponse.AutoLaunchAndReloadPage(protectedOrderRef, sameDevice.BankIdLaunchInfo.LaunchUrl, sameDevice.BankIdLaunchInfo.DeviceMightRequireUserInteractionToLaunchBankIdApp));
            }
            default:
            {
                throw new InvalidOperationException(BankIdConstants.ErrorMessages.UnknownFlowLaunchType);
            }
        }
    }
}
