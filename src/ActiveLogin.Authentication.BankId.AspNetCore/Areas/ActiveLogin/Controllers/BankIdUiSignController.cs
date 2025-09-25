using ActiveLogin.Authentication.BankId.AspNetCore.Cookies;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Sign;
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
public class BankIdUiSignController(
    IAntiforgery antiforgery,
    IStringLocalizer<ActiveLoginResources> localizer,
    IBankIdUserMessageLocalizer bankIdUserMessageLocalizer,
    IBankIdUiOptionsCookieManager uiOptionsCookieManager,
    IBankIdInvalidStateHandler bankIdInvalidStateHandler,
    IStateStorage stateStorage
) : BankIdUiControllerBase<BankIdUiSignState>(antiforgery, localizer, bankIdUserMessageLocalizer, bankIdInvalidStateHandler, uiOptionsCookieManager, stateStorage)
{
    [HttpGet]
    [AllowAnonymous]
    [Route($"/[area]/{BankIdConstants.Routes.BankIdPathName}/{BankIdConstants.Routes.BankIdSignControllerPath}")]
    public Task<ActionResult> Init(string returnUrl, [FromQuery(Name = BankIdConstants.QueryStringParameters.UiOptions)] string protectedUiOptions)
    {
        return Initialize(returnUrl, BankIdConstants.Routes.BankIdSignApiControllerName, protectedUiOptions, "Init");
    }
}
