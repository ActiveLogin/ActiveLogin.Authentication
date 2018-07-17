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
using Microsoft.Extensions.Localization;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.BankIdAuthentication.Controllers
{
    [Area(BankIdAuthenticationConstants.AreaName)]
    [Route("/[area]/Api/")]
    [ApiController]
    public class BankIdApiController : Controller
    {
        private readonly UrlEncoder _urlEncoder;
        private readonly IBankIdUserMessageLocalizer _bankIdUserMessageLocalizer;
        private readonly IBankIdApiClient _bankIdApiClient;
        private readonly IBankIdOrderRefProtector _orderRefProtector;
        private readonly IBankIdLoginResultProtector _loginResultProtector;
        private readonly IBankIdUserMessage _bankIdUserMessage;

        public BankIdApiController(UrlEncoder urlEncoder,
            IBankIdUserMessageLocalizer bankIdUserMessageLocalizer,
            IBankIdApiClient bankIdApiClient,
            IBankIdOrderRefProtector orderRefProtector,
            IBankIdLoginResultProtector loginResultProtector,
            IBankIdUserMessage bankIdUserMessage)
        {
            _urlEncoder = urlEncoder;
            _bankIdUserMessageLocalizer = bankIdUserMessageLocalizer;
            _bankIdApiClient = bankIdApiClient;
            _orderRefProtector = orderRefProtector;
            _loginResultProtector = loginResultProtector;
            _bankIdUserMessage = bankIdUserMessage;
        }

        [ValidateAntiForgeryToken]
        [HttpPost("Initialize")]
        public async Task<ActionResult<BankIdLoginApiInitializeResponse>> InitializeAsync(BankIdLoginApiInitializeRequest request)
        {
            var endUserIp = HttpContext.Connection.RemoteIpAddress.ToString();
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Parse(request.PersonalIdentityNumber);

            var authResponse = await _bankIdApiClient.AuthAsync(endUserIp, personalIdentityNumber.ToLongString());

            var orderRef = authResponse.OrderRef;
            var protectedOrderRef = _orderRefProtector.Protect(new BankIdOrderRef()
            {
                OrderRef = orderRef
            });

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
            var collectResponse = await _bankIdApiClient.CollectAsync(orderRef.OrderRef);
            var statusMessage = GetStatusMessage(collectResponse);

            if (collectResponse.Status == CollectStatus.Pending)
            {
                return Ok(BankIdLoginApiStatusResponse.Pending(statusMessage));
            }

            if (collectResponse.Status == CollectStatus.Complete)
            {
                var returnUri = GetSuccessReturnUri(collectResponse.CompletionData.User, request.ReturnUrl);
                if (!Url.IsLocalUrl(returnUri))
                {
                    throw new Exception(BankIdAuthenticationConstants.InvalidReturnUrlErrorMessage);
                }
                return Ok(BankIdLoginApiStatusResponse.Finished(returnUri));
            }

            return BadRequest(new BankIdLoginApiErrorResponse(statusMessage));
        }

        private string GetStatusMessage(CollectResponse collectResponse)
        {
            //TODO: Set these to correct values
            var authPersonalIdentityNumberProvided = true;
            var accessedFromMobileDevice = true;

            var messageShortName = _bankIdUserMessage.GetMessageShortNameForCollectResponse(collectResponse.Status, collectResponse.HintCode, authPersonalIdentityNumberProvided, accessedFromMobileDevice);
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