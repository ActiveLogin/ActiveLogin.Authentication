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
    private readonly IBankIdSignService _bankIdSignService;

    public SignController(IBankIdSignConfigurationProvider bankIdSignConfigurationProvider, IBankIdSignService bankIdSignService)
    {
        _bankIdSignConfigurationProvider = bankIdSignConfigurationProvider;
        _bankIdSignService = bankIdSignService;
    }

    public async Task<IActionResult> Index()
    {
        var configurations = await _bankIdSignConfigurationProvider.GetAllConfigurationsAsync();
        var providers = configurations
            .Where(x => x.DisplayName != null)
            .Select(x => new ExternalProvider(x.DisplayName ?? x.Key, x.Key));
        var viewModel = new BankIdViewModel(providers, "~/");

        return View(viewModel);
    }

    [AllowAnonymous]
    public IActionResult Sign(string provider)
    {
        var props = new BankIdSignProperties("Sign this...")
        {
            Items =
            {
                {"returnUrl", "~/"},
                {"scheme", provider}
            }
        };
        var returnPath = new PathString(Url.Action(nameof(Callback), new { provider }));
        return _bankIdSignService.InitiateSign(props, returnPath, provider);
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Callback(string provider)
    {
        var result = await _bankIdSignService.GetSignResultAsync(provider);
        if (result?.Succeeded != true)
        {
            throw new Exception("Sign error");
        }

        return Redirect("~/");
    }
}
