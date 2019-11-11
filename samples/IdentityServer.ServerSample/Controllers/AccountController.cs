using System;
using System.Linq;
using System.Threading.Tasks;
using ActiveLogin.Authentication.BankId.AspNetCore;
using IdentityServer.ServerSample.Models;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.ServerSample.Controllers
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

        public async Task<IActionResult> Login(string returnUrl)
        {
            var schemes = await _authenticationSchemeProvider.GetAllSchemesAsync();
            var providers = schemes
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
                    { "scheme", provider },
                }
            };

            if (provider.Equals(BankIdAuthenticationDefaults.SameDeviceAuthenticationScheme))
            {
                props.Items.Add("cancelReturnUrl", Url.ActionLink("Login", "Account", new { returnUrl }));
            }

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

        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            LogoutRequest logoutRequest = await _interaction.GetLogoutContextAsync(logoutId);
            var returnUrl = logoutRequest?.PostLogoutRedirectUri;

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
