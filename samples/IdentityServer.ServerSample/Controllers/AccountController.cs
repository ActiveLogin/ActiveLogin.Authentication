using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.ServerSample.Models;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.ServerSample.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;
        private readonly IIdentityServerInteractionService _interaction;

        public AccountController(IIdentityServerInteractionService interaction,
            IAuthenticationSchemeProvider authenticationSchemeProvider)
        {
            _interaction = interaction;
            _authenticationSchemeProvider = authenticationSchemeProvider;
        }

        public IActionResult Login(string returnUrl)
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
                    { "returnUrl", returnUrl },
                    { "scheme", provider }
                }
            };

            return Challenge(props, provider);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback()
        {
            AuthenticateResult result = await HttpContext.AuthenticateAsync();
            if (result?.Succeeded != true)
            {
                throw new Exception("External authentication error");
            }

            string returnUrl = result.Properties.Items["returnUrl"];

            return _interaction.IsValidReturnUrl(returnUrl) || Url.IsLocalUrl(returnUrl) 
                ? Redirect(returnUrl) 
                : Redirect("~/");
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            LogoutRequest logoutRequest = await _interaction.GetLogoutContextAsync(logoutId);
            string returnUrl = logoutRequest?.PostLogoutRedirectUri;

            return await Logout(new LogoutModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> Logout(LogoutModel model)
        {
            await HttpContext.SignOutAsync();

            return Redirect(model?.ReturnUrl ?? "~/");
        }

        public class LogoutModel
        {
            public string ReturnUrl { get; set; }
        }
    }
}
