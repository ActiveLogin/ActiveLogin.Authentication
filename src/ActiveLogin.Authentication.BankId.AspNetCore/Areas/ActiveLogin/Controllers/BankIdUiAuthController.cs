using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.StateHandling;
using ActiveLogin.Authentication.BankId.Core.UserMessage;

using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Controllers;

[Area(BankIdConstants.Routes.ActiveLoginAreaName)]
[AllowAnonymous]
[NonController]
public class BankIdUiAuthController : BankIdUiControllerBase
{
    public BankIdUiAuthController(
        IAntiforgery antiforgery,
        IStringLocalizer<ActiveLoginResources> localizer,
        IBankIdUserMessageLocalizer bankIdUserMessageLocalizer,
        IBankIdUiOptionsProtector uiOptionsProtector,
        IBankIdInvalidStateHandler bankIdInvalidStateHandler,
        IBankIdUiStateProtector bankIdUiStateProtector
    )
        : base(antiforgery, localizer, bankIdUserMessageLocalizer, uiOptionsProtector, bankIdInvalidStateHandler, bankIdUiStateProtector)
    {

    }

    [HttpGet]
    [Route($"/[area]/{BankIdConstants.Routes.BankIdPathName}/{BankIdConstants.Routes.BankIdAuthControllerPath}")]
    public Task<ActionResult> Init(string returnUrl, [FromQuery(Name = BankIdConstants.QueryStringParameters.UiOptions)] string protectedUiOptions)
    {
        return Initialize(returnUrl, BankIdConstants.Routes.BankIdAuthApiControllerName, protectedUiOptions, "Init");
    }
}
