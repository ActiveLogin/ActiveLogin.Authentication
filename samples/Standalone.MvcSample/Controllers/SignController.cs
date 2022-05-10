using ActiveLogin.Authentication.BankId.AspNetCore.Sign;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Standalone.MvcSample.Models;

namespace Standalone.MvcSample.Controllers;

[AllowAnonymous]
public class SignController : Controller
{
    private readonly IBankIdSignConfigurationProvider _bankIdSignConfigurationProvider;

    public SignController(IBankIdSignConfigurationProvider bankIdSignConfigurationProvider)
    {
        _bankIdSignConfigurationProvider = bankIdSignConfigurationProvider;
    }

    public async Task<IActionResult> Index()
    {
        var configurations = await _bankIdSignConfigurationProvider.GetAllConfigurationsAsync();
        var providers = configurations
            .Where(x => x.DisplayName != null)
            .Select(x => new ExternalProvider(x.DisplayName ?? x.Key, x.Key));
        var viewModel = new AccountLoginViewModel(providers, "~/");

        return View(viewModel);
    }

    [AllowAnonymous]
    public IActionResult Sign(string provider)
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
