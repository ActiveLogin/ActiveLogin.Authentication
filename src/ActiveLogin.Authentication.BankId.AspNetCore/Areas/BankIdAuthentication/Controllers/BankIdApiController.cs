using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.Api.UserMessage;
using ActiveLogin.Authentication.BankId.AspNetCore.Areas.BankIdAuthentication.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Launcher;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Persistence;
using ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice;
using ActiveLogin.Authentication.BankId.AspNetCore.UserMessage;
using ActiveLogin.Identity.Swedish;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.BankIdAuthentication.Controllers
{
    [Area(BankIdAuthenticationConstants.AreaName)]
    [Route("/[area]/Api/")]
    [ApiController]
    [AllowAnonymous]
    public class BankIdApiController : Controller
    {
        private readonly UrlEncoder _urlEncoder;
        private readonly ILogger<BankIdApiController> _logger;
        private readonly IBankIdUserMessage _bankIdUserMessage;
        private readonly IBankIdUserMessageLocalizer _bankIdUserMessageLocalizer;
        private readonly IBankIdSupportedDeviceDetector _bankIdSupportedDeviceDetector;
        private readonly IBankIdLauncher _bankIdLauncher;
        private readonly IBankIdApiClient _bankIdApiClient;
        private readonly IBankIdOrderRefProtector _orderRefProtector;
        private readonly IBankIdLoginOptionsProtector _loginOptionsProtector;
        private readonly IBankIdLoginResultProtector _loginResultProtector;
        private readonly IBankIdResultStore _bankIdResultStore;

        public BankIdApiController(
            UrlEncoder urlEncoder,
            ILogger<BankIdApiController> logger,
            IBankIdUserMessage bankIdUserMessage,
            IBankIdUserMessageLocalizer bankIdUserMessageLocalizer,
            IBankIdSupportedDeviceDetector bankIdSupportedDeviceDetector,
            IBankIdLauncher bankIdLauncher,
            IBankIdApiClient bankIdApiClient,
            IBankIdOrderRefProtector orderRefProtector,
            IBankIdLoginOptionsProtector loginOptionsProtector,
            IBankIdLoginResultProtector loginResultProtector,
            IBankIdResultStore bankIdResultStore)
        {
            _urlEncoder = urlEncoder;
            _logger = logger;
            _bankIdUserMessage = bankIdUserMessage;
            _bankIdUserMessageLocalizer = bankIdUserMessageLocalizer;
            _bankIdSupportedDeviceDetector = bankIdSupportedDeviceDetector;
            _bankIdLauncher = bankIdLauncher;
            _bankIdApiClient = bankIdApiClient;
            _orderRefProtector = orderRefProtector;
            _loginOptionsProtector = loginOptionsProtector;
            _loginResultProtector = loginResultProtector;
            _bankIdResultStore = bankIdResultStore;
        }

        [ValidateAntiForgeryToken]
        [HttpPost("Initialize")]
        public async Task<ActionResult<BankIdLoginApiInitializeResponse>> Initialize(BankIdLoginApiInitializeRequest request)
        {
            var unprotectedLoginOptions = _loginOptionsProtector.Unprotect(request.LoginOptions);

            SwedishPersonalIdentityNumber personalIdentityNumber;
            if (unprotectedLoginOptions.IsAutoLogin())
            {
                personalIdentityNumber = unprotectedLoginOptions.PersonalIdentityNumber;
            }
            else
            {
                if (!SwedishPersonalIdentityNumber.TryParse(request.PersonalIdentityNumber, out personalIdentityNumber))
                {
                    return BadRequest(new
                    {
                        PersonalIdentityNumber = "Invalid PersonalIdentityNumber."
                    });
                }
            }

            AuthResponse authResponse;
            try
            {
                var authRequest = GetAuthRequest(personalIdentityNumber, unprotectedLoginOptions);
                authResponse = await _bankIdApiClient.AuthAsync(authRequest);
            }
            catch (BankIdApiException bankIdApiException)
            {
                _logger.BankIdAuthFailure(personalIdentityNumber, bankIdApiException);

                var errorStatusMessage = GetStatusMessage(bankIdApiException);
                return BadRequest(new BankIdLoginApiErrorResponse(errorStatusMessage));
            }

            var orderRef = authResponse.OrderRef;
            var protectedOrderRef = _orderRefProtector.Protect(new BankIdOrderRef(orderRef));

            _logger.BankIdAuthSuccess(personalIdentityNumber, orderRef);

            if (unprotectedLoginOptions.AutoLaunch)
            {
                var detectedDevice = _bankIdSupportedDeviceDetector.Detect(HttpContext.Request.Headers["User-Agent"]);
                var bankIdRedirectUri = GetBankIdRedirectUri(request, authResponse, detectedDevice);

                var response = detectedDevice.IsIos
                    ? BankIdLoginApiInitializeResponse.AutoLaunch(protectedOrderRef, bankIdRedirectUri, false)
                    : BankIdLoginApiInitializeResponse.AutoLaunchAndCheckStatus(protectedOrderRef, bankIdRedirectUri, detectedDevice.IsAndroid);

                return Ok(response);
            }

            return Ok(BankIdLoginApiInitializeResponse.ManualLaunch(protectedOrderRef));
        }

        private AuthRequest GetAuthRequest(SwedishPersonalIdentityNumber personalIdentityNumber, BankIdLoginOptions loginOptions)
        {
            var endUserIp = GetEndUserIp();
            var personalIdentityNumberString = personalIdentityNumber?.To12DigitString();
            var autoStartTokenRequired = string.IsNullOrEmpty(personalIdentityNumberString) ? true : (bool?)null;

            List<string> certificatePolicies = null;
            if (loginOptions.CertificatePolicies != null && loginOptions.CertificatePolicies.Any())
            {
                certificatePolicies = loginOptions.CertificatePolicies;
            }

            var authRequestRequirement = new Requirement(certificatePolicies, autoStartTokenRequired, loginOptions.AllowBiometric);

            return new AuthRequest(endUserIp, personalIdentityNumberString, authRequestRequirement);
        }

        private string GetEndUserIp()
        {
            return HttpContext.Connection.RemoteIpAddress.ToString();
        }

        private string GetBankIdRedirectUri(BankIdLoginApiInitializeRequest request, AuthResponse authResponse, BankIdSupportedDevice detectedDevice)
        {
            var returnRedirectUri = GetAbsoluteUrl(Url.Action("Login", "BankId", new
            {
                returnUrl = request.ReturnUrl,
                loginOptions = request.LoginOptions
            }));
            var launchUrlRequest = new LaunchUrlRequest(returnRedirectUri, authResponse.AutoStartToken);
            var bankIdRedirectUri = _bankIdLauncher.GetLaunchUrl(detectedDevice, launchUrlRequest);

            return bankIdRedirectUri;
        }

        private string GetAbsoluteUrl(string returnUrl)
        {
            var absoluteUri = $"{Request.Scheme}://{Request.Host.ToUriComponent()}{Request.PathBase.ToUriComponent()}";
            return absoluteUri + returnUrl;
        }

        [ValidateAntiForgeryToken]
        [HttpPost("Status")]
        public async Task<ActionResult> Status(BankIdLoginApiStatusRequest request)
        {
            var unprotectedLoginOptions = _loginOptionsProtector.Unprotect(request.LoginOptions);
            var orderRef = _orderRefProtector.Unprotect(request.OrderRef);

            CollectResponse collectResponse;
            try
            {
                collectResponse = await _bankIdApiClient.CollectAsync(orderRef.OrderRef);
            }
            catch (BankIdApiException bankIdApiException)
            {
                _logger.BankIdCollectFailure(orderRef.OrderRef, bankIdApiException);
                var errorStatusMessage = GetStatusMessage(bankIdApiException);
                return BadRequest(new BankIdLoginApiErrorResponse(errorStatusMessage));
            }

            var statusMessage = GetStatusMessage(collectResponse, unprotectedLoginOptions, HttpContext.Request);

            if (collectResponse.GetCollectStatus() == CollectStatus.Pending)
            {
                return CollectPending(collectResponse, statusMessage);
            }

            if (collectResponse.GetCollectStatus() == CollectStatus.Complete)
            {
                return await CollectComplete(request, collectResponse);
            }

            return CollectFailure(collectResponse, statusMessage);
        }

        private ActionResult CollectFailure(CollectResponse collectResponse, string statusMessage)
        {
            _logger.BankIdCollectFailure(collectResponse.OrderRef, collectResponse.GetCollectHintCode());
            return BadRequest(new BankIdLoginApiErrorResponse(statusMessage));
        }

        private async Task<ActionResult> CollectComplete(BankIdLoginApiStatusRequest request, CollectResponse collectResponse)
        {
            _logger.BankIdCollectCompleted(collectResponse.OrderRef, collectResponse.CompletionData);
            await _bankIdResultStore.StoreCollectCompletedCompletionData(collectResponse.OrderRef, collectResponse.CompletionData);

            var returnUri = GetSuccessReturnUri(collectResponse.CompletionData.User, request.ReturnUrl);
            if (!Url.IsLocalUrl(returnUri))
            {
                throw new Exception(BankIdAuthenticationConstants.InvalidReturnUrlErrorMessage);
            }

            return Ok(BankIdLoginApiStatusResponse.Finished(returnUri));
        }

        private ActionResult CollectPending(CollectResponse collectResponse, string statusMessage)
        {
            _logger.BankIdCollectPending(collectResponse.OrderRef, collectResponse.GetCollectHintCode());
            return Ok(BankIdLoginApiStatusResponse.Pending(statusMessage));
        }

        private string GetStatusMessage(CollectResponse collectResponse, BankIdLoginOptions unprotectedLoginOptions, HttpRequest request)
        {
            var authPersonalIdentityNumberProvided = PersonalIdentityNumberProvided(unprotectedLoginOptions);
            var detectedDevice = _bankIdSupportedDeviceDetector.Detect(request.Headers["User-Agent"]);
            var accessedFromMobileDevice = detectedDevice.IsMobile;

            var messageShortName = _bankIdUserMessage.GetMessageShortNameForCollectResponse(collectResponse.GetCollectStatus(), collectResponse.GetCollectHintCode(), authPersonalIdentityNumberProvided, accessedFromMobileDevice);
            var statusMessage = _bankIdUserMessageLocalizer.GetLocalizedString(messageShortName);

            return statusMessage;
        }

        private static bool PersonalIdentityNumberProvided(BankIdLoginOptions unprotectedLoginOptions)
        {
            return unprotectedLoginOptions.AllowChangingPersonalIdentityNumber
                   || unprotectedLoginOptions.PersonalIdentityNumber != null;
        }

        private string GetStatusMessage(BankIdApiException bankIdApiException)
        {
            var messageShortName = _bankIdUserMessage.GetMessageShortNameForErrorResponse(bankIdApiException.ErrorCode);
            var statusMessage = _bankIdUserMessageLocalizer.GetLocalizedString(messageShortName);

            return statusMessage;
        }

        private string GetSuccessReturnUri(User user, string returnUrl)
        {
            var loginResult = BankIdLoginResult.Success(user.PersonalIdentityNumber, user.Name, user.GivenName, user.Surname);
            var protectedLoginResult = _loginResultProtector.Protect(loginResult);
            var queryString = $"loginResult={_urlEncoder.Encode(protectedLoginResult)}";

            return AppendQueryString(returnUrl, queryString);
        }

        private static string AppendQueryString(string url, string queryString)
        {
            var delimiter = url.Contains("?") ? "&" : "?";
            return $"{url}{delimiter}{queryString}";
        }
    }
}