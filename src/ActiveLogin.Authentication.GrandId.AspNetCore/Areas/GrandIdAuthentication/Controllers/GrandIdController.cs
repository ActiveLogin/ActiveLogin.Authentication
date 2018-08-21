using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ActiveLogin.Authentication.Common.Serialization;
using ActiveLogin.Authentication.GrandId.Api;
using ActiveLogin.Authentication.GrandId.Api.Models;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

namespace ActiveLogin.Authentication.GrandId.AspNetCore.Areas.GrandIdAuthentication.Controllers
{
    [Area(GrandIdAuthenticationConstants.AreaName)]
    [Route("/[area]/[action]")]
    public class GrandIdController : Controller
    {
        private readonly IAntiforgery _antiforgery;
        private readonly IJsonSerializer _jsonSerializer;

        public readonly IGrandIdApiClient _grandIdApiClient;

        public GrandIdController(IAntiforgery antiforgery,
            IJsonSerializer jsonSerializer,
            IGrandIdApiClient grandIdApiClient,
            IGrandIdEnviromentConfiguration enviromentConfiguration)
        {
            _antiforgery = antiforgery;
            _jsonSerializer = jsonSerializer;
            _grandIdApiClient = grandIdApiClient;
            _grandIdApiClient.SetConfiguration(enviromentConfiguration);
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
                var response = await _grandIdApiClient.AuthAsync(deviceOption, returnUrl);
                var redirectUrl = response.RedirectUrl;
                return Redirect(redirectUrl);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static StringContent GetJsonStringContent(string requestJson)
        {
            var requestContent = new StringContent(requestJson, Encoding.Default, "application/json");
            requestContent.Headers.ContentType.CharSet = string.Empty;
            return requestContent;
        }
    }
}