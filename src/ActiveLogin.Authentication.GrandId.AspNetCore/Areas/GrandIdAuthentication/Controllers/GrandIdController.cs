using System;
using System.Threading.Tasks;
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
        public readonly IGrandIdApiClient _grandIdApiClient;

        public GrandIdController(IAntiforgery antiforgery,
            IGrandIdApiClient grandIdApiClient,
            IGrandIdEnviromentConfiguration enviromentConfiguration)
        {
            _antiforgery = antiforgery;
            _grandIdApiClient = grandIdApiClient;
            _grandIdApiClient.SetConfiguration(enviromentConfiguration);
        }
        
        public async Task<ActionResult> Login(string returnUrl)
        {
            if (!Url.IsLocalUrl(returnUrl))
            {
                throw new Exception(GrandIdAuthenticationConstants.InvalidReturnUrlErrorMessage);
            }
            returnUrl = PrepareReturnUrl(returnUrl); // need to add baseAddress for grandId to return to correct page
            var deviceOption = DeviceOption.ChooseDevice; // Todo, how to handle this?

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

        private string PrepareReturnUrl(string returnUrl)
        {
            var absoluteUri = string.Concat(
                     Request.Scheme,
                     "://",
                     Request.Host.ToUriComponent(),
                     Request.PathBase.ToUriComponent());
            return absoluteUri + returnUrl;
        }
    }
}