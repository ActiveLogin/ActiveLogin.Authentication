using System;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ActiveLogin.Authentication.GrandId.Api;
using ActiveLogin.Authentication.GrandId.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ActiveLogin.Authentication.GrandId.AspNetCore.Areas.GrandIdAuthentication.Controllers
{
    [Area(GrandIdAuthenticationConstants.AreaName)]
    [Route("/[area]/[action]")]
    public class GrandIdController : Controller
    {
        private readonly IGrandIdApiClient _grandIdApiClient;
        private readonly UrlEncoder _urlEncoder;
        private readonly ILogger<GrandIdAuthenticationHandler> _logger;

        public GrandIdController(
            IGrandIdApiClient grandIdApiClient,
            UrlEncoder urlEncoder,
            ILogger<GrandIdAuthenticationHandler> logger)
        {
            _grandIdApiClient = grandIdApiClient;
            _urlEncoder = urlEncoder;
            _logger = logger;
        }
        
        public async Task<ActionResult> Login(string returnUrl)
        {
            if (!Url.IsLocalUrl(returnUrl))
            {
                throw new Exception(GrandIdAuthenticationConstants.InvalidReturnUrlErrorMessage);
            }

            var deviceOption = DeviceOption.ChooseDevice; // TODO: How to handle this?
            returnUrl += "?deviceOption=" + _urlEncoder.Encode(deviceOption.ToString());
            var absoluteReturnUrl = GetAbsoluteUrl(returnUrl);

            try
            {
                var response = await _grandIdApiClient.AuthAsync(deviceOption, absoluteReturnUrl);
                var redirectUrl = response.RedirectUrl;
                return Redirect(redirectUrl);
            }
            catch (Exception ex)
            {
                _logger.GrandIdAuthFailure(deviceOption, returnUrl, ex);
                throw;
            }
        }

        private string GetAbsoluteUrl(string returnUrl)
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