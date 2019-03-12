using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Standalone.MvcSample.Models;

namespace Standalone.MvcSample.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;

        public AccountController(IAuthenticationSchemeProvider authenticationSchemeProvider)
        {
            _authenticationSchemeProvider = authenticationSchemeProvider;
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            Task<IEnumerable<AuthenticationScheme>> schemes = _authenticationSchemeProvider.GetAllSchemesAsync();
            IEnumerable<ExternalProvider> providers = schemes.Result
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

        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider)
        {
            var props = new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(ExternalLoginCallback)),
                Items =
                {
                    { "returnUrl", "~/" },
                    { "scheme", provider }
                }
            };

            return Challenge(props, provider);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback()
        {
            AuthenticateResult result = await HttpContext.AuthenticateAsync();
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
