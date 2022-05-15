using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.Api.UserMessage;
using ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Helpers;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Sign;
using ActiveLogin.Authentication.BankId.Core.Flow;
using ActiveLogin.Authentication.BankId.Core.Models;
using ActiveLogin.Authentication.BankId.Core.UserMessage;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Controllers;

[Area(BankIdConstants.Routes.ActiveLoginAreaName)]
[Route($"/[area]/{BankIdConstants.Routes.BankIdPathName}/{BankIdConstants.Routes.BankIdSignControllerPath}/{BankIdConstants.Routes.BankIdApiControllerPath}/")]
[ApiController]
[AllowAnonymous]
[NonController]
public class BankIdUiSignApiController : BankIdUiApiControllerBase
{
    private readonly IBankIdUiStateProtector _bankIdUiStateProtector;

    public BankIdUiSignApiController(
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

        var protectedState = Request.Cookies[uiOptions.StateCookieName];
        if (protectedState == null)
        {
            throw new ArgumentNullException(uiOptions.StateCookieName, "Missing requried state cookie for sign.");
        }
        var state = _bankIdUiStateProtector.Unprotect(protectedState);
        if(state is not BankIdUiSignState signState)
        {
            throw new InvalidOperationException($"State cookie is not created for signing");
        }

        BankIdFlowInitializeResult bankIdFlowInitializeResult;
        try
        {
            var returnRedirectUrl = Url.Action(BankIdConstants.Routes.BankIdSignInitActionName, BankIdConstants.Routes.BankIdSignControllerName, new
            {
                returnUrl = request.ReturnUrl,
                uiOptions = request.UiOptions
            }, protocol: Request.Scheme) ?? throw new Exception(BankIdConstants.ErrorMessages.CouldNotGetUrlFor(BankIdConstants.Routes.BankIdSignControllerName, BankIdConstants.Routes.BankIdSignInitActionName));

            bankIdFlowInitializeResult = await BankIdFlowService.InitializeSign(
                uiOptions.ToBankIdFlowOptions(),
                new BankIdSignData(signState.BankIdSignProperties.UserVisibleData)
                {
                    Items = signState.BankIdSignProperties.Items,
                    UserNonVisibleData = signState.BankIdSignProperties.UserNonVisibleData,
                    UserVisibleDataFormat = signState.BankIdSignProperties.UserVisibleDataFormat
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
}
