using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.Core.StateHandling;
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
        IBankIdUserMessageLocalizer bankIdUserMessageLocalizer,
        IBankIdUiOptionsProtector uiOptionsProtector,
        IStringLocalizer<BankIdHandler> localizer,
        IBankIdInvalidStateHandler bankIdInvalidStateHandler
    )
        : base(antiforgery, bankIdUserMessageLocalizer, uiOptionsProtector, localizer, bankIdInvalidStateHandler)
    {

    }

    [HttpGet]
    [Route($"/[area]/{BankIdConstants.Routes.BankIdPathName}/{BankIdConstants.Routes.BankIdAuthControllerPath}")]
    public Task<ActionResult> Init(string returnUrl, [FromQuery(Name = BankIdConstants.QueryStringParameters.UiOptions)] string protectedUiOptions)
    {
        return Initialize(returnUrl, protectedUiOptions, "Init");
    }
}
