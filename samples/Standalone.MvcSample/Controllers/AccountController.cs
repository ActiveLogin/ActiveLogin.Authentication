using System;
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
        public async Task<IActionResult> Login()
        {
            var schemes = await _authenticationSchemeProvider.GetAllSchemesAsync();
            var providers = schemes
                .Where(x => x.DisplayName != null)
                .Select(x => new ExternalProvider(x.DisplayName ?? x.Name, x.Name));
            var viewModel = new AccountLoginViewModel(providers, "~/");

            return View(viewModel);
        }

        [AllowAnonymous]
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

        [AllowAnonymous]
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
