using ActiveLogin.Authentication.BankId.AspNetCore.Auth;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.StateHandling;
using ActiveLogin.Authentication.BankId.Core;
using ActiveLogin.Authentication.BankId.Core.UserMessage;

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Controllers;

[Area(BankIdConstants.Routes.ActiveLoginAreaName)]
[AllowAnonymous]
[NonController]
public class BankIdUiAuthController(
    IAntiforgery antiforgery,
    IStringLocalizer<ActiveLoginResources> localizer,
    IBankIdUserMessageLocalizer bankIdUserMessageLocalizer,
    IBankIdUiOptionsProtector uiOptionsProtector,
    IBankIdInvalidStateHandler bankIdInvalidStateHandler,
    IStateStorage<BankIdUiAuthState> stateStorage
) : BankIdUiControllerBase<BankIdUiAuthState>(antiforgery, localizer, bankIdUserMessageLocalizer, uiOptionsProtector, bankIdInvalidStateHandler)
{
    [HttpGet]
    [Route($"/[area]/{BankIdConstants.Routes.BankIdPathName}/{BankIdConstants.Routes.BankIdAuthControllerPath}")]
    public Task<ActionResult> Init(string returnUrl, [FromQuery(Name = BankIdConstants.QueryStringParameters.UiOptions)] string protectedUiOptions)
    {
        return Initialize(returnUrl, BankIdConstants.Routes.BankIdAuthApiControllerName, protectedUiOptions, "Init");
    }

    protected override Task<BankIdUiAuthState?> GetUIState(BankIdUiOptions uiOptions)
    {
        var cookie = HttpContext.Request.Cookies[uiOptions.StateKeyCookieName];
        if (cookie is null)
        {
            return Task.FromResult<BankIdUiAuthState?>(null);
        }
        var stateKey = new StateKey(cookie);
        return stateStorage.RemoveAsync(stateKey);
    }
}
