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

            var unprotectedLoginOptions = _loginOptionsProtector.Unprotect(request.LoginOptions);

            InitializeAuthFlowResult initializeAuthFlowResult;
            try
            {
                var returnRedirectLocalUrl = Url.Action("Login", "BankId", new
                {
                    returnUrl = request.ReturnUrl,
                    loginOptions = request.LoginOptions
                });
                var returnRedirectAbsoluteUrl = GetAbsoluteUrl(returnRedirectLocalUrl);
                initializeAuthFlowResult = await _bankIdFlowService.InitializeAuth(unprotectedLoginOptions, returnRedirectAbsoluteUrl);
            }
            catch (BankIdApiException bankIdApiException)
            {
                var errorStatusMessage = GetStatusMessage(bankIdApiException);
                return BadRequestJsonResult(new BankIdLoginApiErrorResponse(errorStatusMessage));
            }

            var protectedOrderRef = _orderRefProtector.Protect(new BankIdOrderRef(initializeAuthFlowResult.BankIdAuthResponse.OrderRef));
            switch (initializeAuthFlowResult.LaunchType)
            {
                case InitializeAuthFlowLaunchTypeOtherDevice otherDeviceLaunchType:
                {
                    var protectedQrStartState = _qrStartStateProtector.Protect(otherDeviceLaunchType.QrStartState);
                    return OkJsonResult(BankIdLoginApiInitializeResponse.ManualLaunch(protectedOrderRef, protectedQrStartState, otherDeviceLaunchType.QrCodeBase64Encoded));
                }
                case InitializeAuthFlowLaunchTypeSameDevice sameDeviceLaunchType when sameDeviceLaunchType.BankIdLaunchInfo.DeviceWillReloadPageOnReturnFromBankIdApp:
                    return OkJsonResult(BankIdLoginApiInitializeResponse.AutoLaunch(protectedOrderRef, sameDeviceLaunchType.BankIdLaunchInfo.LaunchUrl, sameDeviceLaunchType.BankIdLaunchInfo.DeviceMightRequireUserInteractionToLaunchBankIdApp));
                case InitializeAuthFlowLaunchTypeSameDevice sameDeviceLaunchType:
                    return OkJsonResult(BankIdLoginApiInitializeResponse.AutoLaunchAndCheckStatus(protectedOrderRef, sameDeviceLaunchType.BankIdLaunchInfo.LaunchUrl, sameDeviceLaunchType.BankIdLaunchInfo.DeviceMightRequireUserInteractionToLaunchBankIdApp));
                default:
                    throw new InvalidOperationException("Unknown launch type");
            }    
        }

        private string GetAbsoluteUrl(string? returnUrl)
        {
            var absoluteUri = $"{Request.Scheme}://{Request.Host.ToUriComponent()}";
            return absoluteUri + (returnUrl ?? string.Empty);
        }

        [ValidateAntiForgeryToken]
        [HttpPost("Status")]
        public async Task<ActionResult> Status(BankIdLoginApiStatusRequest request)
        {
            ArgumentNullException.ThrowIfNull(request.LoginOptions);
            ArgumentNullException.ThrowIfNull(request.ReturnUrl);
            ArgumentNullException.ThrowIfNull(request.OrderRef);

            var unprotectedLoginOptions = _loginOptionsProtector.Unprotect(request.LoginOptions);
            var orderRef = _orderRefProtector.Unprotect(request.OrderRef);
            var detectedDevice = GetDetectedUserDevice();

            CollectResponse collectResponse;
            try
            {
                collectResponse = await _bankIdApiClient.CollectAsync(orderRef.OrderRef);
            }
            catch (BankIdApiException bankIdApiException)
            {
                await _bankIdEventTrigger.TriggerAsync(new BankIdCollectErrorEvent(orderRef.OrderRef, bankIdApiException, detectedDevice, unprotectedLoginOptions));
                var errorStatusMessage = GetStatusMessage(bankIdApiException);
                return BadRequestJsonResult(new BankIdLoginApiErrorResponse(errorStatusMessage));
            }

            var statusMessage = GetStatusMessage(collectResponse, unprotectedLoginOptions, detectedDevice);

            if (collectResponse.GetCollectStatus() == CollectStatus.Pending)
            {
                return await CollectPending(collectResponse, statusMessage, detectedDevice, unprotectedLoginOptions);
            }

            if (collectResponse.GetCollectStatus() == CollectStatus.Complete)
            {
                return await CollectComplete(request, collectResponse, detectedDevice, unprotectedLoginOptions);
            }

            var hintCode = collectResponse.GetCollectHintCode();
            if (hintCode.Equals(CollectHintCode.StartFailed)
                && request.AutoStartAttempts < BankIdConstants.MaxRetryLoginAttempts)
            {
                return OkJsonResult(BankIdLoginApiStatusResponse.Retry(statusMessage));
            }

            return await CollectFailure(collectResponse, statusMessage, detectedDevice, unprotectedLoginOptions);
        }

        private async Task<ActionResult> CollectFailure(CollectResponse collectResponse, string statusMessage, BankIdSupportedDevice detectedDevice, BankIdLoginOptions loginOptions)
        {
            await _bankIdEventTrigger.TriggerAsync(new BankIdCollectFailureEvent(collectResponse.OrderRef, collectResponse.GetCollectHintCode(), detectedDevice, loginOptions));
            return BadRequestJsonResult(new BankIdLoginApiErrorResponse(statusMessage));
        }

        private async Task<ActionResult> CollectComplete(BankIdLoginApiStatusRequest request, CollectResponse collectResponse, BankIdSupportedDevice detectedDevice, BankIdLoginOptions loginOptions)
        {
            if (collectResponse.CompletionData == null)
            {
                throw new ArgumentNullException(nameof(collectResponse.CompletionData));
            }

            if (request.ReturnUrl == null)
            {
                throw new ArgumentNullException(nameof(request.ReturnUrl));
            }

            await _bankIdEventTrigger.TriggerAsync(new BankIdCollectCompletedEvent(collectResponse.OrderRef, collectResponse.CompletionData, detectedDevice, loginOptions));

            var returnUri = GetSuccessReturnUri(collectResponse.OrderRef, collectResponse.CompletionData.User, request.ReturnUrl);
            if (!Url.IsLocalUrl(returnUri))
            {
                throw new Exception(BankIdConstants.InvalidReturnUrlErrorMessage);
            }

            return OkJsonResult(BankIdLoginApiStatusResponse.Finished(returnUri));
        }

        private async Task<ActionResult> CollectPending(CollectResponse collectResponse, string statusMessage, BankIdSupportedDevice detectedDevice, BankIdLoginOptions loginOptions)
        {
            await _bankIdEventTrigger.TriggerAsync(new BankIdCollectPendingEvent(collectResponse.OrderRef, collectResponse.GetCollectHintCode(), detectedDevice, loginOptions));
            return OkJsonResult(BankIdLoginApiStatusResponse.Pending(statusMessage));
        }

        private string GetStatusMessage(CollectResponse collectResponse, BankIdLoginOptions unprotectedLoginOptions, BankIdSupportedDevice detectedDevice)
        {
            var accessedFromMobileDevice = detectedDevice.DeviceType == BankIdSupportedDeviceType.Mobile;
            var usingQrCode = !unprotectedLoginOptions.SameDevice;

            var messageShortName = _bankIdUserMessage.GetMessageShortNameForCollectResponse(
                collectResponse.GetCollectStatus(),
                collectResponse.GetCollectHintCode(),
                authPersonalIdentityNumberProvided: false,
                accessedFromMobileDevice,
                usingQrCode);
            var statusMessage = _bankIdUserMessageLocalizer.GetLocalizedString(messageShortName);

            return statusMessage;
        }

        private BankIdSupportedDevice GetDetectedUserDevice()
        {
            return _bankIdSupportedDeviceDetector.Detect();
        }

        private string GetStatusMessage(BankIdApiException bankIdApiException)
        {
            var messageShortName = _bankIdUserMessage.GetMessageShortNameForErrorResponse(bankIdApiException.ErrorCode);
            var statusMessage = _bankIdUserMessageLocalizer.GetLocalizedString(messageShortName);

            return statusMessage;
        }

        private string GetSuccessReturnUri(string orderRef, User user, string returnUrl)
        {
            var loginResult = BankIdLoginResult.Success(orderRef, user.PersonalIdentityNumber, user.Name, user.GivenName, user.Surname);
            var protectedLoginResult = _loginResultProtector.Protect(loginResult);
            var queryString = $"loginResult={_urlEncoder.Encode(protectedLoginResult)}";

            return AppendQueryString(returnUrl, queryString);
        }

        private static string AppendQueryString(string url, string queryString)
        {
            var delimiter = url.Contains('?') ? "&" : "?";
            return $"{url}{delimiter}{queryString}";
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
            var detectedDevice = GetDetectedUserDevice();

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
