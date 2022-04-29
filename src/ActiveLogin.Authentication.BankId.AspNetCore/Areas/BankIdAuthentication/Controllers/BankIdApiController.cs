using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.Api.UserMessage;
using ActiveLogin.Authentication.BankId.AspNetCore.Areas.BankIdAuthentication.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Events;
using ActiveLogin.Authentication.BankId.AspNetCore.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice;
using ActiveLogin.Authentication.BankId.AspNetCore.UserMessage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ActiveLogin.Authentication.BankId.AspNetCore.Flow;
using Microsoft.AspNetCore.WebUtilities;
using System.Collections.Generic;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.BankIdAuthentication.Controllers
{
    [Area(BankIdConstants.AreaName)]
    [Route("/[area]/Api/")]
    [ApiController]
    [AllowAnonymous]
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
        [HttpPost("Initialize")]
        public async Task<ActionResult<BankIdLoginApiInitializeResponse>> Initialize(BankIdLoginApiInitializeRequest request)
        {
            ArgumentNullException.ThrowIfNull(request.ReturnUrl, nameof(request.ReturnUrl));
            ArgumentNullException.ThrowIfNull(request.LoginOptions, nameof(request.LoginOptions));

            var loginOptions = _loginOptionsProtector.Unprotect(request.LoginOptions);

            BankIdFlowInitializeAuthResult bankIdFlowInitializeAuthResult;
            try
            {
                var returnRedirectUrl = Url.Action("Login", "BankId", new
                {
                    returnUrl = request.ReturnUrl,
                    loginOptions = request.LoginOptions
                },  protocol: Request.Scheme) ?? throw new Exception($"Could not get URL for BankId.Login");

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
                    throw new InvalidOperationException("Unknown launch type");
                }
            }    
        }

        [ValidateAntiForgeryToken]
        [HttpPost("Status")]
        public async Task<ActionResult> Status(BankIdLoginApiStatusRequest request)
        {
            ArgumentNullException.ThrowIfNull(request.OrderRef, nameof(request.OrderRef));
            ArgumentNullException.ThrowIfNull(request.ReturnUrl, nameof(request.ReturnUrl));
            ArgumentNullException.ThrowIfNull(request.LoginOptions, nameof(request.LoginOptions));

            if (!Url.IsLocalUrl(request.ReturnUrl))
            {
                throw new Exception(BankIdConstants.InvalidReturnUrlErrorMessage);
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
                    throw new InvalidOperationException("Unknown collect result type");
                }
            }
        }

        [ValidateAntiForgeryToken]
        [HttpPost("QrCode")]
        public ActionResult QrCode(BankIdLoginApiQrCodeRequest request)
        {
            ArgumentNullException.ThrowIfNull(request.QrStartState, nameof(request.QrStartState));

            var qrStartState = _qrStartStateProtector.Unprotect(request.QrStartState);
            var qrCodeAsBase64 = _bankIdFlowService.GetQrCodeAsBase64(qrStartState);

            return OkJsonResult(new BankIdLoginApiQrCodeResponse(qrCodeAsBase64));
        }

        [ValidateAntiForgeryToken]
        [HttpPost("Cancel")]
        public async Task<ActionResult> Cancel(BankIdLoginApiCancelRequest request)
        {
            ArgumentNullException.ThrowIfNull(request.OrderRef, nameof(request.OrderRef));
            ArgumentNullException.ThrowIfNull(request.LoginOptions, nameof(request.LoginOptions));

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
                { "loginResult", protectedLoginResult }
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
}
