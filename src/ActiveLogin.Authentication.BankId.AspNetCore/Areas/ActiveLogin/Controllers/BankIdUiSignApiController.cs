using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.Api.UserMessage;
using ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Cookies;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Helpers;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Sign;
using ActiveLogin.Authentication.BankId.Core;
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
public class BankIdUiSignApiController(
    IBankIdFlowService bankIdFlowService,
    IBankIdDataStateProtector<BankIdUiOrderRef> orderRefProtector,
    IBankIdDataStateProtector<BankIdQrStartState> qrStartStateProtector,
    IBankIdUserMessage bankIdUserMessage,
    IBankIdUserMessageLocalizer bankIdUserMessageLocalizer,
    IBankIdDataStateProtector<BankIdUiResult> uiAuthResultProtector,
    IBankIdUiOptionsCookieManager uiOptionsCookieManager,
    IStateStorage stateStorage
) : BankIdUiApiControllerBase(bankIdFlowService, orderRefProtector, qrStartStateProtector, uiOptionsCookieManager, bankIdUserMessage, bankIdUserMessageLocalizer, uiAuthResultProtector, stateStorage)
{
    [ValidateAntiForgeryToken]
    [HttpPost(BankIdConstants.Routes.BankIdApiInitializeActionName)]
    public async Task<ActionResult<BankIdUiApiInitializeResponse>> Initialize(BankIdUiApiInitializeRequest request)
    {
        Validators.ThrowIfNullOrWhitespace(request.ReturnUrl, nameof(request.ReturnUrl));
        Validators.ThrowIfNullOrWhitespace(request.UiOptions, nameof(request.UiOptions));

        var uiOptions = ResolveProtectedUiOptions(request.UiOptions);

        var state = await GetState<BankIdUiSignState>(uiOptions) ?? throw new InvalidOperationException(BankIdConstants.ErrorMessages.InvalidStateCookie);
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
                new BankIdSignData(state.BankIdSignProperties.UserVisibleData)
                {
                    Items = state.BankIdSignProperties.Items,
                    UserNonVisibleData = state.BankIdSignProperties.UserNonVisibleData,
                    UserVisibleDataFormat = state.BankIdSignProperties.UserVisibleDataFormat,
                    RequiredPersonalIdentityNumber = state.BankIdSignProperties.RequiredPersonalIdentityNumber,
                    RequireMrtd = state.BankIdSignProperties.RequireMrtd,
                    RequirePinCode = state.BankIdSignProperties.RequirePinCode,
                    CertificatePolicies = state.BankIdSignProperties.BankIdCertificatePolicies,
                    CardReader = state.BankIdSignProperties.CardReader,
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
