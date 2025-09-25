using ActiveLogin.Authentication.BankId.AspNetCore.Auth;
using ActiveLogin.Authentication.BankId.AspNetCore.Cookies;
using ActiveLogin.Authentication.BankId.AspNetCore.StateHandling;
using ActiveLogin.Authentication.BankId.Core;
using ActiveLogin.Authentication.BankId.Core.UserMessage;

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
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
    IBankIdInvalidStateHandler bankIdInvalidStateHandler,
    IBankIdUiOptionsCookieManager uiOptionsCookieManager,
    IStateStorage stateStorage
) : BankIdUiControllerBase<BankIdUiAuthState>(antiforgery, localizer, bankIdUserMessageLocalizer, bankIdInvalidStateHandler, uiOptionsCookieManager, stateStorage)
{
    [HttpGet]
    [Route($"/[area]/{BankIdConstants.Routes.BankIdPathName}/{BankIdConstants.Routes.BankIdAuthControllerPath}")]
    public Task<ActionResult> Init(string returnUrl, [FromQuery(Name = BankIdConstants.QueryStringParameters.UiOptions)] string uiOptionsGuid)
    {
        return Initialize(returnUrl, BankIdConstants.Routes.BankIdAuthApiControllerName, uiOptionsGuid, "Init");
    }
}
