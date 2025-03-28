using ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Sign;
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
public class BankIdUiSignController : BankIdUiControllerBase<BankIdUiSignState>
{
    public BankIdUiSignController(
        IAntiforgery antiforgery,
        IStringLocalizer<ActiveLoginResources> localizer,
        IBankIdUserMessageLocalizer bankIdUserMessageLocalizer,
        IBankIdUiOptionsProtector uiOptionsProtector,
        IBankIdInvalidStateHandler bankIdInvalidStateHandler,
        IStateStorage stateStorage
    ) : base(antiforgery, localizer, bankIdUserMessageLocalizer, uiOptionsProtector, bankIdInvalidStateHandler, stateStorage)
    {
    }

    [HttpGet]
    [AllowAnonymous]
    [Route($"/[area]/{BankIdConstants.Routes.BankIdPathName}/{BankIdConstants.Routes.BankIdSignControllerPath}")]
    public Task<ActionResult> Init(string returnUrl, [FromQuery(Name = BankIdConstants.QueryStringParameters.UiOptions)] string protectedUiOptions, [FromQuery(Name = "nonce")] string nonce)
    {
        return Initialize(returnUrl, BankIdConstants.Routes.BankIdSignApiControllerName, protectedUiOptions, nonce, "Init");
    }
}
