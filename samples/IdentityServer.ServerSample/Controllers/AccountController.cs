using IdentityServer.ServerSample.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Net;
namespace IdentityServer.ServerSample.Controllers;

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
            .Select(x => new ExternalProvider(x.DisplayName ?? x.Name, x.Name));
        var sanitizedReturnUrl = System.Net.WebUtility.HtmlEncode(returnUrl);
        var viewModel = new AccountLoginViewModel(providers, sanitizedReturnUrl);

        return View(viewModel);
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
                { "cancelReturnUrl", Url.Action("Login", "Account", new { returnUrl }) }
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

        var returnUrl = result.Properties?.Items["returnUrl"];

        if (returnUrl != null && _interaction.IsValidReturnUrl(returnUrl) || Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return Redirect("~/");
    }

    [HttpGet]
    public async Task<IActionResult> Logout(string logoutId)
    {
        var logoutRequest = await _interaction.GetLogoutContextAsync(logoutId);
        var returnUrl = logoutRequest?.PostLogoutRedirectUri;

        return await Logout(new LogoutModel(returnUrl));
    }

    [HttpPost]
    public async Task<IActionResult> Logout(LogoutModel model)
    {
        await HttpContext.SignOutAsync();

        return Redirect(model?.ReturnUrl ?? "~/");
    }

    public class LogoutModel
    {
        public LogoutModel() : this(null)
        {
        }

        public LogoutModel(string? returnUrl)
        {
            ReturnUrl = returnUrl;
        }

        public string? ReturnUrl { get; }
    }
}
