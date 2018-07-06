using System;
using ActiveLogin.Authentication.BankId.AspNetCore.Areas.BankIdAuthentication.Models;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.BankIdAuthentication.Controllers
{
    [Area(BankIdAuthenticationConstants.AreaName)]
    [Route("/[area]/[action]")]
    public class BankIdController : Controller
    {
        private readonly IAntiforgery _antiforgery;

        public BankIdController(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }
    
        public ActionResult Login(string returnUrl)
        {
            if (!Url.IsLocalUrl(returnUrl))
            {
                throw new Exception(BankIdAuthenticationConstants.InvalidReturnUrlErrorMessage);
            }

            var antiforgeryTokens = _antiforgery.GetAndStoreTokens(HttpContext);
            return View(new BankIdLoginViewModel
            {
                ReturnUrl = returnUrl,
                AntiXsrfRequestToken = antiforgeryTokens.RequestToken,
                LoginScriptOptions = new BankIdLoginScriptOptions()
                {
                    RefreshIntervalMs = BankIdAuthenticationDefaults.StatusRefreshIntervalMs,
                    InitialStatusMessage = "", //TODO: Set initial status

                    BankIdInitializeApiUrl = Url.Action(nameof(BankIdApiController.Initialize), "BankIdApi"),
                    BankIdStatusApiUrl = Url.Action(nameof(BankIdApiController.Status), "BankIdApi")
                }
            });
        }
    }
}