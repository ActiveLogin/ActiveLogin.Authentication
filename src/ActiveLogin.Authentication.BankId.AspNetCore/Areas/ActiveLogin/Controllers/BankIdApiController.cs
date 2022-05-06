using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.Api.UserMessage;
using ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Helpers;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.Core.Flow;
using ActiveLogin.Authentication.BankId.Core.UserMessage;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Controllers;

[Area(BankIdConstants.Routes.ActiveLoginAreaName)]
[Route($"/[area]/{BankIdConstants.Routes.BankIdPathName}/{BankIdConstants.Routes.BankIdApiControllerPath}/")]
[ApiController]
[AllowAnonymous]
[NonController]
public class BankIdApiController : Controller
{
    private readonly IBankIdFlowService _bankIdFlowService;

    private readonly IBankIdUserMessage _bankIdUserMessage;
    private readonly IBankIdUserMessageLocalizer _bankIdUserMessageLocalizer;

    private readonly IBankIdOrderRefProtector _orderRefProtector;
    private readonly IBankIdQrStartStateProtector _qrStartStateProtector;
    private readonly IBankIdLoginOptionsProtector _loginOptionsProtector;
    private readonly IBankIdLoginResultProtector _loginResultProtector;

    public BankIdApiController(
        IBankIdFlowService bankIdFlowService,

        IBankIdUserMessage bankIdUserMessage,
        IBankIdUserMessageLocalizer bankIdUserMessageLocalizer,

        IBankIdOrderRefProtector orderRefProtector,
        IBankIdQrStartStateProtector qrStartStateProtector,
        IBankIdLoginOptionsProtector loginOptionsProtector,
        IBankIdLoginResultProtector loginResultProtector

    )
    {
        _bankIdFlowService = bankIdFlowService;

        _bankIdUserMessage = bankIdUserMessage;
        _bankIdUserMessageLocalizer = bankIdUserMessageLocalizer;

        _orderRefProtector = orderRefProtector;
        _qrStartStateProtector = qrStartStateProtector;
        _loginOptionsProtector = loginOptionsProtector;
        _loginResultProtector = loginResultProtector;
    }

    [ValidateAntiForgeryToken]
    [HttpPost(BankIdConstants.Routes.BankIdApiInitializeActionName)]
    public async Task<ActionResult<BankIdLoginApiInitializeResponse>> Initialize(BankIdLoginApiInitializeRequest request)
    {
        Validators.ThrowIfNullOrWhitespace(request.ReturnUrl, nameof(request.ReturnUrl));
        Validators.ThrowIfNullOrWhitespace(request.LoginOptions, nameof(request.LoginOptions));

        var loginOptions = _loginOptionsProtector.Unprotect(request.LoginOptions);

        BankIdFlowInitializeAuthResult bankIdFlowInitializeAuthResult;
        try
        {
            var returnRedirectUrl = Url.Action(BankIdConstants.Routes.BankIdAuthInitActionName, BankIdConstants.Routes.BankIdAuthControllerName, new
            {
                returnUrl = request.ReturnUrl,
                loginOptions = request.LoginOptions
            },  protocol: Request.Scheme) ?? throw new Exception(BankIdConstants.ErrorMessages.CouldNotGetUrlFor(BankIdConstants.Routes.BankIdAuthControllerName, BankIdConstants.Routes.BankIdAuthInitActionName));

            bankIdFlowInitializeAuthResult = await _bankIdFlowService.InitializeAuth(loginOptions, returnRedirectUrl);
        }
        catch (BankIdApiException bankIdApiException)
        {
            var errorStatusMessage = GetBankIdApiExceptionStatusMessage(bankIdApiException);
            return BadRequestJsonResult(new BankIdLoginApiErrorResponse(errorStatusMessage));
        }

        var protectedOrderRef = _orderRefProtector.Protect(new BankIdOrderRef(bankIdFlowInitializeAuthResult.BankIdAuthResponse.OrderRef));
        switch (bankIdFlowInitializeAuthResult.LaunchType)
        {
            case BankIdFlowInitializeAuthLaunchTypeOtherDevice otherDevice:
            {
                var protectedQrStartState = _qrStartStateProtector.Protect(otherDevice.QrStartState);
                return OkJsonResult(BankIdLoginApiInitializeResponse.ManualLaunch(protectedOrderRef, protectedQrStartState, otherDevice.QrCodeBase64Encoded));
            }
            case BankIdFlowInitializeAuthLaunchTypeSameDevice sameDevice when sameDevice.BankIdLaunchInfo.DeviceWillReloadPageOnReturnFromBankIdApp:
            {
                return OkJsonResult(BankIdLoginApiInitializeResponse.AutoLaunch(protectedOrderRef, sameDevice.BankIdLaunchInfo.LaunchUrl, sameDevice.BankIdLaunchInfo.DeviceMightRequireUserInteractionToLaunchBankIdApp));
            }
            case BankIdFlowInitializeAuthLaunchTypeSameDevice sameDevice:
            {
                return OkJsonResult(BankIdLoginApiInitializeResponse.AutoLaunchAndCheckStatus(protectedOrderRef, sameDevice.BankIdLaunchInfo.LaunchUrl, sameDevice.BankIdLaunchInfo.DeviceMightRequireUserInteractionToLaunchBankIdApp));
            }
            default:
            {
                throw new InvalidOperationException(BankIdConstants.ErrorMessages.UnknownFlowLaunchType);
            }
        }    
    }

    [ValidateAntiForgeryToken]
    [HttpPost(BankIdConstants.Routes.BankIdApiStatusActionName)]
    public async Task<ActionResult> Status(BankIdLoginApiStatusRequest request)
    {
        Validators.ThrowIfNullOrWhitespace(request.OrderRef, nameof(request.OrderRef));
        Validators.ThrowIfNullOrWhitespace(request.ReturnUrl, nameof(request.ReturnUrl));
        Validators.ThrowIfNullOrWhitespace(request.LoginOptions, nameof(request.LoginOptions));

        if (!Url.IsLocalUrl(request.ReturnUrl))
        {
            throw new Exception(BankIdConstants.ErrorMessages.InvalidReturnUrl);
        }

        var orderRef = _orderRefProtector.Unprotect(request.OrderRef);
        var loginOptions = _loginOptionsProtector.Unprotect(request.LoginOptions);

        BankIdFlowCollectResult result;
        try
        {
            result = await _bankIdFlowService.Collect(orderRef.OrderRef, request.AutoStartAttempts, loginOptions);
        }
        catch (BankIdApiException bankIdApiException)
        {
            var errorStatusMessage = GetBankIdApiExceptionStatusMessage(bankIdApiException);
            return BadRequestJsonResult(new BankIdLoginApiErrorResponse(errorStatusMessage));
        }

        switch(result)
        {
            case BankIdFlowCollectResultPending pending:
            {
                return OkJsonResult(BankIdLoginApiStatusResponse.Pending(pending.StatusMessage));
            }
            case BankIdFlowCollectResultComplete complete:
            {
                var returnUri = GetSuccessReturnUrl(orderRef.OrderRef, complete.CompletionData.User, request.ReturnUrl);
                return OkJsonResult(BankIdLoginApiStatusResponse.Finished(returnUri));
            }
            case BankIdFlowCollectResultRetry retry:
            {
                return OkJsonResult(BankIdLoginApiStatusResponse.Retry(retry.StatusMessage));
            }
            case BankIdFlowCollectResultFailure failure:
            {
                return BadRequestJsonResult(new BankIdLoginApiErrorResponse(failure.StatusMessage));
            }
            default:
            {
                throw new InvalidOperationException(BankIdConstants.ErrorMessages.UnknownFlowCollectResultType);
            }
        }
    }

    [ValidateAntiForgeryToken]
    [HttpPost(BankIdConstants.Routes.BankIdApiQrCodeActionName)]
    public ActionResult QrCode(BankIdLoginApiQrCodeRequest request)
    {
        Validators.ThrowIfNullOrWhitespace(request.QrStartState, nameof(request.QrStartState));

        var qrStartState = _qrStartStateProtector.Unprotect(request.QrStartState);
        var qrCodeAsBase64 = _bankIdFlowService.GetQrCodeAsBase64(qrStartState);

        return OkJsonResult(new BankIdLoginApiQrCodeResponse(qrCodeAsBase64));
    }

    [ValidateAntiForgeryToken]
    [HttpPost(BankIdConstants.Routes.BankIdApiCancelActionName)]
    public async Task<ActionResult> Cancel(BankIdLoginApiCancelRequest request)
    {
        Validators.ThrowIfNullOrWhitespace(request.OrderRef, nameof(request.OrderRef));
        Validators.ThrowIfNullOrWhitespace(request.LoginOptions, nameof(request.LoginOptions));

        var orderRef = _orderRefProtector.Unprotect(request.OrderRef);
        var loginOptions = _loginOptionsProtector.Unprotect(request.LoginOptions);

        await _bankIdFlowService.Cancel(orderRef.OrderRef, loginOptions);

        return OkJsonResult(BankIdLoginApiCancelResponse.Cancelled());
    }

    private string GetBankIdApiExceptionStatusMessage(BankIdApiException bankIdApiException)
    {
        var messageShortName = _bankIdUserMessage.GetMessageShortNameForErrorResponse(bankIdApiException.ErrorCode);
        var statusMessage = _bankIdUserMessageLocalizer.GetLocalizedString(messageShortName);

        return statusMessage;
    }

    private string GetSuccessReturnUrl(string orderRef, User user, string returnUrl)
    {
        var loginResult = BankIdLoginResult.Success(orderRef, user.PersonalIdentityNumber, user.Name, user.GivenName, user.Surname);
        var protectedLoginResult = _loginResultProtector.Protect(loginResult);

        return QueryHelpers.AddQueryString(returnUrl, new Dictionary<string, string?>
        {
            { BankIdConstants.QueryStringParameters.LoginResult, protectedLoginResult }
        });
    }

    private static ActionResult OkJsonResult(object model)
    {
        return new JsonResult(model, BankIdConstants.JsonSerializerOptions)
        {
            StatusCode = StatusCodes.Status200OK
        };
    }

    private static ActionResult BadRequestJsonResult(object model)
    {
        return new JsonResult(model, BankIdConstants.JsonSerializerOptions)
        {
            StatusCode = StatusCodes.Status400BadRequest
        };
    }
}
