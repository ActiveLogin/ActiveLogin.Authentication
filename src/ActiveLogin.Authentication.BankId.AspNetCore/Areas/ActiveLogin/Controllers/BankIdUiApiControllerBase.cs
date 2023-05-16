using System.Net.Mime;
using System.Text.Json;

using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.Api.UserMessage;
using ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Helpers;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.Core.Flow;
using ActiveLogin.Authentication.BankId.Core.UserMessage;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Controllers;

[NonController]
public abstract class BankIdUiApiControllerBase : ControllerBase
{
    private static JsonSerializerOptions JsonSerializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    protected readonly IBankIdFlowService BankIdFlowService;
    protected readonly IBankIdUiOrderRefProtector OrderRefProtector;
    protected readonly IBankIdQrStartStateProtector QrStartStateProtector;
    protected readonly IBankIdUiOptionsProtector UiOptionsProtector;

    private readonly IBankIdUserMessage _bankIdUserMessage;
    private readonly IBankIdUserMessageLocalizer _bankIdUserMessageLocalizer;
    private readonly IBankIdUiResultProtector _uiAuthResultProtector;

    protected BankIdUiApiControllerBase(
        IBankIdFlowService bankIdFlowService,
        IBankIdUiOrderRefProtector orderRefProtector,
        IBankIdQrStartStateProtector qrStartStateProtector,
        IBankIdUiOptionsProtector uiOptionsProtector,

        IBankIdUserMessage bankIdUserMessage,
        IBankIdUserMessageLocalizer bankIdUserMessageLocalizer,
        IBankIdUiResultProtector uiAuthResultProtector

    )
    {
        BankIdFlowService = bankIdFlowService;
        OrderRefProtector = orderRefProtector;
        QrStartStateProtector = qrStartStateProtector;
        UiOptionsProtector = uiOptionsProtector;

        _bankIdUserMessage = bankIdUserMessage;
        _bankIdUserMessageLocalizer = bankIdUserMessageLocalizer;
        _uiAuthResultProtector = uiAuthResultProtector;
    }

    [ValidateAntiForgeryToken]
    [HttpPost(BankIdConstants.Routes.BankIdApiStatusActionName)]
    public async Task<ActionResult> Status(BankIdUiApiStatusRequest request)
    {
        Validators.ThrowIfNullOrWhitespace(request.OrderRef, nameof(request.OrderRef));
        Validators.ThrowIfNullOrWhitespace(request.ReturnUrl, nameof(request.ReturnUrl));
        Validators.ThrowIfNullOrWhitespace(request.UiOptions, nameof(request.UiOptions));

        if (!Url.IsLocalUrl(request.ReturnUrl))
        {
            throw new Exception(BankIdConstants.ErrorMessages.InvalidReturnUrl);
        }

        var orderRef = OrderRefProtector.Unprotect(request.OrderRef);
        var uiOptions = UiOptionsProtector.Unprotect(request.UiOptions);

        BankIdFlowCollectResult result;
        try
        {
            result = await BankIdFlowService.Collect(orderRef.OrderRef, request.AutoStartAttempts, uiOptions.ToBankIdFlowOptions());
        }
        catch (BankIdApiException bankIdApiException)
        {
            var errorStatusMessage = GetBankIdApiExceptionStatusMessage(bankIdApiException);
            return BadRequestJsonResult(new BankIdUiApiErrorResponse(errorStatusMessage));
        }

        switch(result)
        {
            case BankIdFlowCollectResultPending pending:
            {
                return OkJsonResult(BankIdUiApiStatusResponse.Pending(pending.StatusMessage));
            }
            case BankIdFlowCollectResultComplete complete:
            {
                var uiResult = ConstructProtectedUiResult(orderRef.OrderRef, complete.CompletionData);
                return OkJsonResult(BankIdUiApiStatusResponse.Finished(request.ReturnUrl, uiResult));
            }
            case BankIdFlowCollectResultRetry retry:
            {
                return OkJsonResult(BankIdUiApiStatusResponse.Retry(retry.StatusMessage));
            }
            case BankIdFlowCollectResultFailure failure:
            {
                return BadRequestJsonResult(new BankIdUiApiErrorResponse(failure.StatusMessage));
            }
            default:
            {
                throw new InvalidOperationException(BankIdConstants.ErrorMessages.UnknownFlowCollectResultType);
            }
        }
    }

    [ValidateAntiForgeryToken]
    [HttpPost(BankIdConstants.Routes.BankIdApiQrCodeActionName)]
    public ActionResult QrCode(BankIdUiApiQrCodeRequest request)
    {
        Validators.ThrowIfNullOrWhitespace(request.QrStartState, nameof(request.QrStartState));

        var qrStartState = QrStartStateProtector.Unprotect(request.QrStartState);
        var qrCodeAsBase64 = BankIdFlowService.GetQrCodeAsBase64(qrStartState);

        return OkJsonResult(new BankIdUiApiQrCodeResponse(qrCodeAsBase64));
    }

    [ValidateAntiForgeryToken]
    [HttpPost(BankIdConstants.Routes.BankIdApiCancelActionName)]
    public async Task<ActionResult> Cancel(BankIdUiApiCancelRequest request)
    {
        Validators.ThrowIfNullOrWhitespace(request.OrderRef, nameof(request.OrderRef));
        Validators.ThrowIfNullOrWhitespace(request.UiOptions, nameof(request.UiOptions));

        var orderRef = OrderRefProtector.Unprotect(request.OrderRef);
        var uiOptions = UiOptionsProtector.Unprotect(request.UiOptions);

        await BankIdFlowService.Cancel(orderRef.OrderRef, uiOptions.ToBankIdFlowOptions());

        return OkJsonResult(BankIdUiCancelResponse.Cancelled());
    }

    protected string GetBankIdApiExceptionStatusMessage(BankIdApiException bankIdApiException)
    {
        var messageShortName = _bankIdUserMessage.GetMessageShortNameForErrorResponse(bankIdApiException.ErrorCode);
        var statusMessage = _bankIdUserMessageLocalizer.GetLocalizedString(messageShortName);

        return statusMessage;
    }

    protected string ConstructProtectedUiResult(string orderRef, CompletionData completionData)
    {
        var user = completionData.User;
        var uiResult = BankIdUiResult.Success(
            orderRef,
            user.PersonalIdentityNumber,
            user.Name,
            user.GivenName,
            user.Surname,
            completionData.BankIdIssueDate,
            completionData.StepUp.Mrtd,
            completionData.Signature,
            completionData.OcspResponse,
            completionData.Device.IpAddress,
            completionData.Device.Uhi);
        return _uiAuthResultProtector.Protect(uiResult);
    }

    protected static ActionResult OkJsonResult(object model)
    {
        return new ContentResult
        {
            ContentType = MediaTypeNames.Application.Json,
            StatusCode = StatusCodes.Status200OK,
            Content = JsonSerializer.Serialize(model, JsonSerializerOptions)
        };
    }

    protected static ActionResult BadRequestJsonResult(object model)
    {
        return new ContentResult
        {
            ContentType = MediaTypeNames.Application.Json,
            StatusCode = StatusCodes.Status400BadRequest,
            Content = JsonSerializer.Serialize(model, JsonSerializerOptions)
        };
    }
}
