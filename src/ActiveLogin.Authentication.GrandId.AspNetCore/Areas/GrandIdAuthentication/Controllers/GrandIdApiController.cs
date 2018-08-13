using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ActiveLogin.Authentication.GrandId.Api;
using ActiveLogin.Authentication.GrandId.Api.Models;
using ActiveLogin.Authentication.GrandId.Api.UserMessage;
using ActiveLogin.Authentication.GrandId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.GrandId.AspNetCore.Models;
using ActiveLogin.Authentication.GrandId.AspNetCore.Persistence;
using ActiveLogin.Authentication.GrandId.AspNetCore.Resources;
using ActiveLogin.Identity.Swedish;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ActiveLogin.Authentication.GrandId.AspNetCore.Areas.GrandIdAuthentication.Controllers
{
    [Area(GrandIdAuthenticationConstants.AreaName)]
    [Route("/[area]/Api/")]
    [ApiController]
    public class GrandIdApiController : Controller
    {
        private readonly UrlEncoder _urlEncoder;
        private readonly ILogger<GrandIdApiController> _logger;
        private readonly IGrandIdUserMessage _bankIdUserMessage;
        private readonly IGrandIdUserMessageLocalizer _bankIdUserMessageLocalizer;
        private readonly IGrandIdApiClient _bankIdApiClient;
        private readonly IGrandIdOrderRefProtector _orderRefProtector;
        private readonly IGrandIdLoginResultProtector _loginResultProtector;
        private readonly IGrandIdResultStore _bankIdResultStore;

        public GrandIdApiController(
            UrlEncoder urlEncoder,
            ILogger<GrandIdApiController> logger,
            IGrandIdUserMessage bankIdUserMessage,
            IGrandIdUserMessageLocalizer bankIdUserMessageLocalizer,
            IGrandIdApiClient bankIdApiClient,
            IGrandIdOrderRefProtector orderRefProtector,
            IGrandIdLoginResultProtector loginResultProtector,
            IGrandIdResultStore bankIdResultStore)
        {
            _urlEncoder = urlEncoder;
            _logger = logger;
            _bankIdUserMessage = bankIdUserMessage;
            _bankIdUserMessageLocalizer = bankIdUserMessageLocalizer;
            _bankIdApiClient = bankIdApiClient;
            _orderRefProtector = orderRefProtector;
            _loginResultProtector = loginResultProtector;
            _bankIdResultStore = bankIdResultStore;
        }

        [ValidateAntiForgeryToken]
        [HttpPost("Initialize")]
        public async Task<ActionResult<GrandIdLoginApiInitializeResponse>> InitializeAsync(GrandIdLoginApiInitializeRequest request)
        {
            var endUserIp = HttpContext.Connection.RemoteIpAddress.ToString();
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Parse(request.PersonalIdentityNumber);

            AuthResponse authResponse;
            try
            {
                authResponse = await _bankIdApiClient.AuthAsync(endUserIp, personalIdentityNumber.ToLongString());
            }
            catch (GrandIdApiException bankIdApiException)
            {
                _logger.GrandIdAuthFailure(personalIdentityNumber, bankIdApiException);

                var errorStatusMessage = GetStatusMessage(bankIdApiException);
                return BadRequest(new GrandIdLoginApiErrorResponse(errorStatusMessage));
            }

            var orderRef = authResponse.OrderRef;
            var protectedOrderRef = _orderRefProtector.Protect(new GrandIdOrderRef()
            {
                OrderRef = orderRef
            });

            _logger.GrandIdAuthSuccess(personalIdentityNumber, orderRef);

            return Ok(new GrandIdLoginApiInitializeResponse
            {
                OrderRef = protectedOrderRef
            });
        }

        [ValidateAntiForgeryToken]
        [HttpPost("Status")]
        public async Task<ActionResult> StatusAsync(GrandIdLoginApiStatusRequest request)
        {
            var orderRef = _orderRefProtector.Unprotect(request.OrderRef);
            CollectResponse collectResponse;
            try
            {
                collectResponse = await _bankIdApiClient.CollectAsync(orderRef.OrderRef);
            }
            catch (GrandIdApiException bankIdApiException)
            {
                _logger.GrandIdCollectFailure(orderRef.OrderRef, bankIdApiException);
                var errorStatusMessage = GetStatusMessage(bankIdApiException);
                return BadRequest(new GrandIdLoginApiErrorResponse(errorStatusMessage));
            }

            var statusMessage = GetStatusMessage(collectResponse);

            if (collectResponse.Status == CollectStatus.Pending)
            {
                _logger.GrandIdCollectPending(collectResponse.OrderRef, collectResponse.HintCode);
                return Ok(GrandIdLoginApiStatusResponse.Pending(statusMessage));
            }

            if (collectResponse.Status == CollectStatus.Complete)
            {
                _logger.GrandIdCollectCompleted(collectResponse.OrderRef, collectResponse.CompletionData);
                await _bankIdResultStore.StoreCollectCompletedCompletionData(collectResponse.OrderRef, collectResponse.CompletionData);

                var returnUri = GetSuccessReturnUri(collectResponse.CompletionData.User, request.ReturnUrl);
                if (!Url.IsLocalUrl(returnUri))
                {
                    throw new Exception(GrandIdAuthenticationConstants.InvalidReturnUrlErrorMessage);
                }

                return Ok(GrandIdLoginApiStatusResponse.Finished(returnUri));
            }

            _logger.GrandIdCollectFailure(collectResponse.OrderRef, collectResponse.HintCode);
            return BadRequest(new GrandIdLoginApiErrorResponse(statusMessage));
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

        private string GetStatusMessage(GrandIdApiException bankIdApiException)
        {
            var messageShortName = _bankIdUserMessage.GetMessageShortNameForErrorResponse(bankIdApiException.ErrorCode);
            var statusMessage = _bankIdUserMessageLocalizer.GetLocalizedString(messageShortName);

            return statusMessage;
        }

        private string GetSuccessReturnUri(User user, string returnUrl)
        {
            var loginResult = GrandIdLoginResult.Success(user.PersonalIdentityNumber, user.Name, user.GivenName, user.Surname);
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