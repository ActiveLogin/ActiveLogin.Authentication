using System;
using ActiveLogin.Authentication.BankId.Api.UserMessage;
using ActiveLogin.Authentication.BankId.AspNetCore.Areas.BankIdAuthentication.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Resources;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.BankIdAuthentication.Controllers
{
    [Area(BankIdAuthenticationConstants.AreaName)]
    [Route("/[area]/[action]")]
    public class BankIdController : Controller
    {
        private readonly IAntiforgery _antiforgery;
        private readonly IBankIdUserMessageLocalizer _bankIdUserMessageLocalizer;

        public BankIdController(IAntiforgery antiforgery, IBankIdUserMessageLocalizer bankIdUserMessageLocalizer)
        {
            _antiforgery = antiforgery;
            _bankIdUserMessageLocalizer = bankIdUserMessageLocalizer;
        }
    
        public ActionResult Login(string returnUrl, string loginOptions)
        {
            if (!Url.IsLocalUrl(returnUrl))
            {
                throw new Exception(BankIdAuthenticationConstants.InvalidReturnUrlErrorMessage);
            }

            var antiforgeryTokens = _antiforgery.GetAndStoreTokens(HttpContext);
            return View(new BankIdLoginViewModel
            {
                ReturnUrl = returnUrl,
                LoginOptions = loginOptions,
                AntiXsrfRequestToken = antiforgeryTokens.RequestToken,
                LoginScriptOptions = new BankIdLoginScriptOptions()
                {
                    RefreshIntervalMs = BankIdAuthenticationDefaults.StatusRefreshIntervalMs,

                    InitialStatusMessage = _bankIdUserMessageLocalizer.GetLocalizedString(MessageShortName.RFA1),
                    UnknownErrorMessage = _bankIdUserMessageLocalizer.GetLocalizedString(MessageShortName.RFA22),

                    BankIdInitializeApiUrl = Url.Action(nameof(BankIdApiController.InitializeAsync), "BankIdApi"),
                    BankIdStatusApiUrl = Url.Action(nameof(BankIdApiController.StatusAsync), "BankIdApi")
                }
            });
        }
    }
}