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
        private readonly IGrandIdApiClient _grandIdApiClient;
        private readonly IGrandIdOrderRefProtector _orderRefProtector;
        private readonly IGrandIdLoginResultProtector _loginResultProtector;
        private readonly IGrandIdResultStore _bankIdResultStore;

        public GrandIdApiController(
            UrlEncoder urlEncoder,
            IGrandIdApiClient grandIdApiClient,
            IGrandIdOrderRefProtector orderRefProtector,
            IGrandIdLoginResultProtector loginResultProtector)
        {
            _urlEncoder = urlEncoder;
            _grandIdApiClient = grandIdApiClient;
            _orderRefProtector = orderRefProtector;
            _loginResultProtector = loginResultProtector;
        }

        //[ValidateAntiForgeryToken]
        [HttpPost("Initialize")]
        public async Task<ActionResult<GrandIdLoginApiInitializeResponse>> InitializeAsync(GrandIdLoginApiInitializeRequest request)
        {
            var apiKey = ""; // Todo: handle with configuration
            // Todo move to helper to fetch values from config
            var deviceOptionKey = "";

            switch (request.DeviceOption)
            {
                case DeviceOption.SameDevice:
                    deviceOptionKey = ""; // Todo: handle with configuration
                    break;
                case DeviceOption.OtherDevice:
                    deviceOptionKey = ""; // Todo: handle with configuration
                    break;
                case DeviceOption.ChooseDevice:
                    deviceOptionKey = ""; // Todo: handle with configuration
                    break;
            }
            AuthResponse authResponse;
            try
            {
                authResponse = await _grandIdApiClient.AuthAsync(apiKey, deviceOptionKey, request.ReturnUrl);
            }
            catch (GrandIdApiException grandIdApiException)
            {

                return BadRequest(new GrandIdLoginApiErrorResponse(grandIdApiException.Message));
            }
            
            return Ok(new GrandIdLoginApiInitializeResponse
            {
                SessionId = authResponse.SessionId,
                RedirectUrl = authResponse.RedirectUrl
            });
        }

        //[ValidateAntiForgeryToken]
        [HttpPost("Session")]
        public async Task<ActionResult<GrandIdLoginApiSessionResponse>> GetSessionAsync(GrandIdLoginApiSessionRequest request)
        {
            var apiKey = ""; // Todo: handle with configuration
            // Todo move to helper to fetch values from config
            var deviceOptionKey = "";

            switch (request.DeviceOption)
            {
                case DeviceOption.SameDevice:
                    deviceOptionKey = ""; // Todo: handle with configuration
                    break;
                case DeviceOption.OtherDevice:
                    deviceOptionKey = ""; // Todo: handle with configuration
                    break;
                case DeviceOption.ChooseDevice:
                    deviceOptionKey = ""; // Todo: handle with configuration
                    break;
            }
            SessionStateResponse sessionStateResponse;
            try
            {
                sessionStateResponse = await _grandIdApiClient.GetSessionAsync(apiKey, deviceOptionKey, request.SessionId);
            }
            catch (GrandIdApiException grandIdApiException)
            {

                return BadRequest(new GrandIdLoginApiErrorResponse(grandIdApiException.Message));
            }

            return Ok(new GrandIdLoginApiSessionResponse
            {
                SessionId = sessionStateResponse.SessionId,
                UserAttributes = sessionStateResponse.UserAttributes
            });
        }
    }
}