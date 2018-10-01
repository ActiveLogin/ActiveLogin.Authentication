using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Services;
using IdentityServerSample.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServerSample.Controllers
{
    public class AccountController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;

        public AccountController(IIdentityServerInteractionService interaction,
            IAuthenticationSchemeProvider authenticationSchemeProvider)
        {
            _interaction = interaction;
            _authenticationSchemeProvider = authenticationSchemeProvider;
        }

        public IActionResult Login(string returnUrl)
        {
            var schemes = _authenticationSchemeProvider.GetAllSchemesAsync();
            var providers = schemes.Result
                .Where(x => x.DisplayName != null)
                .Select(x => new ExternalProvider
                {
                    DisplayName = x.DisplayName,
                    AuthenticationScheme = x.Name
                });

            return View(new AccountLoginViewModel
            {
                ExternalProviders = providers,
                ReturnUrl = returnUrl
            });
        }

        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(ExternalLoginCallback)),
                Items =
                {
                    {"returnUrl", returnUrl},
                    {"scheme", provider}
                }
            };

            return Challenge(props, provider);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback()
        {
            var result = await HttpContext.AuthenticateAsync();
            if (result?.Succeeded != true)
            {
                throw new Exception("External authentication error");
            }

            var returnUrl = result.Properties.Items["returnUrl"];

            if (_interaction.IsValidReturnUrl(returnUrl) || Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect("~/");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("~/");
        }
    }
}