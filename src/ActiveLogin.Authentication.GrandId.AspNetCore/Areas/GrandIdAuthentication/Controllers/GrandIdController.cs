using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ActiveLogin.Authentication.Common.Serialization;
using ActiveLogin.Authentication.GrandId.Api;
using ActiveLogin.Authentication.GrandId.Api.Models;
using ActiveLogin.Authentication.GrandId.AspNetCore.Models;
using ActiveLogin.Authentication.GrandId.AspNetCore.Resources;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

namespace ActiveLogin.Authentication.GrandId.AspNetCore.Areas.GrandIdAuthentication.Controllers
{
    [Area(GrandIdAuthenticationConstants.AreaName)]
    [Route("/[area]/[action]")]
    public class GrandIdController : Controller
    {
        private readonly IAntiforgery _antiforgery;
        private readonly IGrandIdUserMessageLocalizer _bankIdUserMessageLocalizer;
        private readonly IJsonSerializer _jsonSerializer;

        public readonly IGrandIdApiClient _grandIdApiClient;
        private readonly IGrandIdEnviromentConfiguration _enviromentConfiguration;

        public GrandIdController(IAntiforgery antiforgery,
            IGrandIdUserMessageLocalizer grandIdUserMessageLocalizer,
            IJsonSerializer jsonSerializer,
            IGrandIdApiClient grandIdApiClient,
            IGrandIdEnviromentConfiguration enviromentConfiguration)
        {
            _antiforgery = antiforgery;
            _bankIdUserMessageLocalizer = grandIdUserMessageLocalizer;
            _jsonSerializer = jsonSerializer;
            _grandIdApiClient = grandIdApiClient;
            _enviromentConfiguration = enviromentConfiguration;
        }

        public async Task<ActionResult> Login(string returnUrl)
        {
            //if (!Url.IsLocalUrl(returnUrl))
            //{
            //    throw new Exception(GrandIdAuthenticationConstants.InvalidReturnUrlErrorMessage);
            //}
            var deviceOption = Api.Models.DeviceOption.ChooseDevice; // Todo, how to handle this?

            returnUrl += "?deviceOption=" + deviceOption.ToString();
           
            try
            {
                var authRequest = GetRequest(deviceOption, returnUrl);
                var response = await _grandIdApiClient.AuthAsync(authRequest);
                var redirectUrl = response.RedirectUrl;
                return Redirect(redirectUrl);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private AuthRequest GetRequest(DeviceOption deviceOption, string callBackUrl)
        {
            var apiKey = _enviromentConfiguration.ApiKey; // Todo: handle with configuration
            // Todo move to helper to fetch values from config
            var deviceOptionKey = _enviromentConfiguration.GetDeviceOptionKey(deviceOption);

            return new AuthRequest(apiKey, deviceOptionKey, callBackUrl);
        }

        private static StringContent GetJsonStringContent(string requestJson)
        {
            var requestContent = new StringContent(requestJson, Encoding.Default, "application/json");
            requestContent.Headers.ContentType.CharSet = string.Empty;
            return requestContent;
        }
    }
}