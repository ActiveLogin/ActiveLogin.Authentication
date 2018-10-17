using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.Api.UserMessage;
using ActiveLogin.Authentication.BankId.AspNetCore.Areas.BankIdAuthentication.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Persistence;
using ActiveLogin.Authentication.BankId.AspNetCore.Resources;
using ActiveLogin.Authentication.BankId.AspNetCore.UserMessage;
using ActiveLogin.Identity.Swedish;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.BankIdAuthentication.Controllers
{
    [Area(BankIdAuthenticationConstants.AreaName)]
    [Route("/[area]/Api/")]
    [ApiController]
    public class BankIdApiController : Controller
    {
        private readonly UrlEncoder _urlEncoder;
        private readonly ILogger<BankIdApiController> _logger;
        private readonly IBankIdUserMessage _bankIdUserMessage;
        private readonly IBankIdUserMessageLocalizer _bankIdUserMessageLocalizer;
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
            _bankIdApiClient = bankIdApiClient;
            _orderRefProtector = orderRefProtector;
            _loginOptionsProtector = loginOptionsProtector;
            _loginResultProtector = loginResultProtector;
            _bankIdResultStore = bankIdResultStore;
        }

        [ValidateAntiForgeryToken]
        [HttpPost("Initialize")]
        public async Task<ActionResult<BankIdLoginApiInitializeResponse>> InitializeAsync(BankIdLoginApiInitializeRequest request)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Parse(request.PersonalIdentityNumber);
            var loginOptions = _loginOptionsProtector.Unprotect(request.LoginOptions);

            AuthResponse authResponse;
            try
            {
                var authRequest = GetAuthRequest(personalIdentityNumber, loginOptions);
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

            return Ok(new BankIdLoginApiInitializeResponse(protectedOrderRef));
        }

        private AuthRequest GetAuthRequest(SwedishPersonalIdentityNumber personalIdentityNumber, BankIdLoginOptions loginOptions)
        {
            var endUserIp = GetEndUserIp();
            var authRequestRequirement = new Requirement(loginOptions.CertificatePolicies);
            var authRequest = new AuthRequest(endUserIp, personalIdentityNumber.ToLongString(), authRequestRequirement);

            return authRequest;
        }

        private string GetEndUserIp()
        {
            return HttpContext.Connection.RemoteIpAddress.ToString();
        }

        [ValidateAntiForgeryToken]
        [HttpPost("Status")]
        public async Task<ActionResult> StatusAsync(BankIdLoginApiStatusRequest request)
        {
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

            var statusMessage = GetStatusMessage(collectResponse);

            if (collectResponse.Status == CollectStatus.Pending)
            {
                return CollectPending(collectResponse, statusMessage);
            }

            if (collectResponse.Status == CollectStatus.Complete)
            {
                return await CollectComplete(request, collectResponse);
            }

            return CollectFailure(collectResponse, statusMessage);
        }

        private ActionResult CollectFailure(CollectResponse collectResponse, string statusMessage)
        {
            _logger.BankIdCollectFailure(collectResponse.OrderRef, collectResponse.HintCode);
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
            _logger.BankIdCollectPending(collectResponse.OrderRef, collectResponse.HintCode);
            return Ok(BankIdLoginApiStatusResponse.Pending(statusMessage));
        }

        private string GetStatusMessage(CollectResponse collectResponse)
        {
            //TODO: Set these to correct values: maybe device detection in combinatio with properties from LoginOptions
            var authPersonalIdentityNumberProvided = true;
            var accessedFromMobileDevice = true;

            var messageShortName = _bankIdUserMessage.GetMessageShortNameForCollectResponse(collectResponse.Status, collectResponse.HintCode, authPersonalIdentityNumberProvided, accessedFromMobileDevice);
            var statusMessage = _bankIdUserMessageLocalizer.GetLocalizedString(messageShortName);

            return statusMessage;
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