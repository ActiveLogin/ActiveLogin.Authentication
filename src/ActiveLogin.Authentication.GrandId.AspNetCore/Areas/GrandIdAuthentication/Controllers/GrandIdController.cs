using System;
using System.Threading.Tasks;
using ActiveLogin.Authentication.GrandId.Api;
using ActiveLogin.Authentication.GrandId.Api.Models;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ActiveLogin.Authentication.GrandId.AspNetCore.Areas.GrandIdAuthentication.Controllers
{
    [Area(GrandIdAuthenticationConstants.AreaName)]
    [Route("/[area]/[action]")]
    public class GrandIdController : Controller
    {
        private readonly IGrandIdApiClient _grandIdApiClient;
        private readonly ILogger<GrandIdAuthenticationHandler> _logger;

        public GrandIdController(IAntiforgery antiforgery,
            IGrandIdApiClient grandIdApiClient,
            IGrandIdEnviromentConfiguration enviromentConfiguration,
            ILogger<GrandIdAuthenticationHandler> logger)
        {
            _grandIdApiClient = grandIdApiClient;
            _logger = logger;
        }
        
        public async Task<ActionResult> Login(string returnUrl)
        {
            if (!Url.IsLocalUrl(returnUrl))
            {
                throw new Exception(GrandIdAuthenticationConstants.InvalidReturnUrlErrorMessage);
            }
            returnUrl = PrepareReturnUrl(returnUrl); // need to add baseAddress for grandId to return to correct page
            var deviceOption = DeviceOption.ChooseDevice; // Todo, how to handle this?

            returnUrl += "?deviceOption=" + deviceOption;

            try
            {
                var response = await _grandIdApiClient.AuthAsync(deviceOption, returnUrl);
                var redirectUrl = response.RedirectUrl;
                return Redirect(redirectUrl);
            }
            catch (GrandIdApiException grandIdApiException)
            {
                _logger.LogError(grandIdApiException, $"Error requesting redirectUrl for {deviceOption} : {grandIdApiException.ErrorCode}-{ grandIdApiException.Details}");
                throw new Exception("Something went wrong when initializing login, please contact the administrator if the problem persists");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error requesting redirectUrl for '{DeviceOption}'", deviceOption);
                throw;
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