using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.Api.UserMessage;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Resources;
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
        private readonly IBankIdLoginResultProtector _loginResultProtector;

        public BankIdApiController(UrlEncoder urlEncoder,
            ILogger<BankIdApiController> logger,
            IBankIdUserMessage bankIdUserMessage,
            IBankIdUserMessageLocalizer bankIdUserMessageLocalizer,
            IBankIdApiClient bankIdApiClient,
            IBankIdOrderRefProtector orderRefProtector,
            IBankIdLoginResultProtector loginResultProtector)
        {
            _urlEncoder = urlEncoder;
            _logger = logger;
            _bankIdUserMessage = bankIdUserMessage;
            _bankIdUserMessageLocalizer = bankIdUserMessageLocalizer;
            _bankIdApiClient = bankIdApiClient;
            _orderRefProtector = orderRefProtector;
            _loginResultProtector = loginResultProtector;
        }

        [ValidateAntiForgeryToken]
        [HttpPost("Initialize")]
        public async Task<ActionResult<BankIdLoginApiInitializeResponse>> InitializeAsync(BankIdLoginApiInitializeRequest request)
        {
            var endUserIp = HttpContext.Connection.RemoteIpAddress.ToString();
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Parse(request.PersonalIdentityNumber);

            AuthResponse authResponse;
            try
            {
                authResponse = await _bankIdApiClient.AuthAsync(endUserIp, personalIdentityNumber.ToLongString());
            }
            catch (BankIdApiException bankIdApiException)
            {
                _logger.BankIdAuthFailure(personalIdentityNumber, bankIdApiException);

                var errorStatusMessage = GetStatusMessage(bankIdApiException);
                return BadRequest(new BankIdLoginApiErrorResponse(errorStatusMessage));
            }

            var orderRef = authResponse.OrderRef;
            var protectedOrderRef = _orderRefProtector.Protect(new BankIdOrderRef()
            {
                OrderRef = orderRef
            });

            _logger.BankIdAuthSuccess(personalIdentityNumber, orderRef);

            return Ok(new BankIdLoginApiInitializeResponse
            {
                OrderRef = protectedOrderRef
            });
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
                _logger.BankIdCollectPending(collectResponse.OrderRef, collectResponse.HintCode);
                return Ok(BankIdLoginApiStatusResponse.Pending(statusMessage));
            }

            if (collectResponse.Status == CollectStatus.Complete)
            {
                _logger.BankIdCollectCompleted(collectResponse.OrderRef, collectResponse.CompletionData);

                var returnUri = GetSuccessReturnUri(collectResponse.CompletionData.User, request.ReturnUrl);
                if (!Url.IsLocalUrl(returnUri))
                {
                    throw new Exception(BankIdAuthenticationConstants.InvalidReturnUrlErrorMessage);
                }
                return Ok(BankIdLoginApiStatusResponse.Finished(returnUri));
            }

            _logger.BankIdCollectFailure(collectResponse.OrderRef, collectResponse.HintCode);
            return BadRequest(new BankIdLoginApiErrorResponse(statusMessage));
        }

        private string GetStatusMessage(CollectResponse collectResponse)
        {
            //TODO: Set these to correct values, might be provided from option / depending on what requirements are set
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
            var fullUrl = url;
            if (!fullUrl.Contains("?"))
            {
                fullUrl += '?';
            }
            else
            {
                fullUrl += '&';
            }
            fullUrl += queryString;

            return fullUrl;
        }
    }
}