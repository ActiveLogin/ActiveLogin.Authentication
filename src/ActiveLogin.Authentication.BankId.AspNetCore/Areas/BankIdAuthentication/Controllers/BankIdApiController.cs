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
        private readonly UrlEncoder _urlEncoder;
        private readonly IBankIdUserMessage _bankIdUserMessage;
        private readonly IBankIdUserMessageLocalizer _bankIdUserMessageLocalizer;
        private readonly IBankIdSupportedDeviceDetector _bankIdSupportedDeviceDetector;
        private readonly IBankIdApiClient _bankIdApiClient;
        private readonly IBankIdOrderRefProtector _orderRefProtector;
        private readonly IBankIdQrStartStateProtector _qrStartStateProtector;
        private readonly IBankIdLoginOptionsProtector _loginOptionsProtector;
        private readonly IBankIdLoginResultProtector _loginResultProtector;
        private readonly IBankIdEventTrigger _bankIdEventTrigger;
        private readonly IBankIdFlowService _bankIdFlowService;

        public BankIdApiController(
            UrlEncoder urlEncoder,
            IBankIdUserMessage bankIdUserMessage,
            IBankIdUserMessageLocalizer bankIdUserMessageLocalizer,
            IBankIdSupportedDeviceDetector bankIdSupportedDeviceDetector,
            IBankIdApiClient bankIdApiClient,
            IBankIdOrderRefProtector orderRefProtector,
            IBankIdQrStartStateProtector qrStartStateProtector,
            IBankIdLoginOptionsProtector loginOptionsProtector,
            IBankIdLoginResultProtector loginResultProtector,
            IBankIdEventTrigger bankIdEventTrigger,
            IBankIdFlowService bankIdFlowService)
        {
            _urlEncoder = urlEncoder;
            _bankIdUserMessage = bankIdUserMessage;
            _bankIdUserMessageLocalizer = bankIdUserMessageLocalizer;
            _bankIdSupportedDeviceDetector = bankIdSupportedDeviceDetector;
            _bankIdApiClient = bankIdApiClient;
            _orderRefProtector = orderRefProtector;
            _qrStartStateProtector = qrStartStateProtector;
            _loginOptionsProtector = loginOptionsProtector;
            _loginResultProtector = loginResultProtector;
            _bankIdEventTrigger = bankIdEventTrigger;
            _bankIdFlowService = bankIdFlowService;
        }

        [ValidateAntiForgeryToken]
        [HttpPost("Initialize")]
        public async Task<ActionResult<BankIdLoginApiInitializeResponse>> Initialize(BankIdLoginApiInitializeRequest request)
        {
            ArgumentNullException.ThrowIfNull(request.LoginOptions, nameof(request.LoginOptions));
            ArgumentNullException.ThrowIfNull(request.ReturnUrl, nameof(request.ReturnUrl));

            var loginOptions = _loginOptionsProtector.Unprotect(request.LoginOptions);

            InitializeAuthFlowResult initializeAuthFlowResult;
            try
            {
                var returnRedirectUrl = Url.Action("Login", "BankId", new
                {
                    returnUrl = request.ReturnUrl,
                    loginOptions = request.LoginOptions
                },
                protocol: Request.Scheme) ?? throw new Exception($"Could not get URL for BankId.Login");

                initializeAuthFlowResult = await _bankIdFlowService.InitializeAuth(loginOptions, returnRedirectUrl);
            }
            catch (BankIdApiException bankIdApiException)
            {
                var errorStatusMessage = GetBankIdApiExceptionStatusMessage(bankIdApiException);
                return BadRequestJsonResult(new BankIdLoginApiErrorResponse(errorStatusMessage));
            }

            var protectedOrderRef = _orderRefProtector.Protect(new BankIdOrderRef(initializeAuthFlowResult.BankIdAuthResponse.OrderRef));
            switch (initializeAuthFlowResult.LaunchType)
            {
                case InitializeAuthFlowLaunchTypeOtherDevice otherDevice:
                {
                    var protectedQrStartState = _qrStartStateProtector.Protect(otherDevice.QrStartState);
                    return OkJsonResult(BankIdLoginApiInitializeResponse.ManualLaunch(protectedOrderRef, protectedQrStartState, otherDevice.QrCodeBase64Encoded));
                }
                case InitializeAuthFlowLaunchTypeSameDevice sameDevice when sameDevice.BankIdLaunchInfo.DeviceWillReloadPageOnReturnFromBankIdApp:
                {
                    return OkJsonResult(BankIdLoginApiInitializeResponse.AutoLaunch(protectedOrderRef, sameDevice.BankIdLaunchInfo.LaunchUrl, sameDevice.BankIdLaunchInfo.DeviceMightRequireUserInteractionToLaunchBankIdApp));
                }
                case InitializeAuthFlowLaunchTypeSameDevice sameDevice:
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
            ArgumentNullException.ThrowIfNull(request.LoginOptions, nameof(request.LoginOptions));
            ArgumentNullException.ThrowIfNull(request.ReturnUrl, nameof(request.ReturnUrl));
            ArgumentNullException.ThrowIfNull(request.OrderRef, nameof(request.OrderRef));

            if (!Url.IsLocalUrl(request.ReturnUrl))
            {
                throw new Exception(BankIdConstants.InvalidReturnUrlErrorMessage);
            }

            var loginOptions = _loginOptionsProtector.Unprotect(request.LoginOptions);
            var orderRef = _orderRefProtector.Unprotect(request.OrderRef);

            BankIdFlowCollectResult result;
            try
            {
                result = await _bankIdFlowService.TryCollect(orderRef.OrderRef, request.AutoStartAttempts, loginOptions);
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

        [ValidateAntiForgeryToken]
        [HttpPost("QrCode")]
        public ActionResult QrCode(BankIdLoginApiQrCodeRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.QrStartState))
            {
                throw new ArgumentNullException(nameof(request.QrStartState));
            }

            var unprotectedQrStartState = _qrStartStateProtector.Unprotect(request.QrStartState);
            var qrCodeAsBase64 = _bankIdFlowService.GetQrCode(unprotectedQrStartState);

            return OkJsonResult(new BankIdLoginApiQrCodeResponse(qrCodeAsBase64));
        }

        [ValidateAntiForgeryToken]
        [HttpPost("Cancel")]
        public async Task<ActionResult> Cancel(BankIdLoginApiCancelRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.LoginOptions))
            {
                throw new ArgumentNullException(nameof(request.LoginOptions));
            }

            if (request.OrderRef == null)
            {
                throw new ArgumentNullException(nameof(request.OrderRef));
            }

            var unprotectedLoginOptions = _loginOptionsProtector.Unprotect(request.LoginOptions);
            var orderRef = _orderRefProtector.Unprotect(request.OrderRef);
            var detectedDevice = _bankIdSupportedDeviceDetector.Detect();

            try
            {
                await _bankIdApiClient.CancelAsync(orderRef.OrderRef);
                await _bankIdEventTrigger.TriggerAsync(new BankIdCancelSuccessEvent(orderRef.OrderRef, detectedDevice, unprotectedLoginOptions));
            }
            catch (BankIdApiException exception)
            {
                // When we get exception in a cancellation request, chances
                // are that the orderRef has already been cancelled or we have
                // a network issue. We still want to provide the GUI with the
                // validated cancellation URL though.
                await _bankIdEventTrigger.TriggerAsync(new BankIdCancelErrorEvent(orderRef.OrderRef, exception, detectedDevice, unprotectedLoginOptions));
            }

            return OkJsonResult(BankIdLoginApiCancelResponse.Cancelled());
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
