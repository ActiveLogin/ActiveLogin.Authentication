using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using StandaloneMvcClientSample.Models;

namespace StandaloneMvcClientSample.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;

        public AccountController(IAuthenticationSchemeProvider authenticationSchemeProvider)
        {
            _authenticationSchemeProvider = authenticationSchemeProvider;
        }

        public IActionResult Login()
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
                ReturnUrl = "~/"
            });
        }

        public IActionResult ExternalLogin(string provider)
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(ExternalLoginCallback)),
                Items =
                {
                    {"returnUrl", "~/"},
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

            return Redirect("~/");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return Redirect("~/");
        }
    }
}