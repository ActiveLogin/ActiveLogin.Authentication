using System;
using ActiveLogin.Authentication.GrandId.Api.UserMessage;
using ActiveLogin.Authentication.GrandId.AspNetCore.Areas.GrandIdAuthentication.Models;
using ActiveLogin.Authentication.GrandId.AspNetCore.Resources;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;

namespace ActiveLogin.Authentication.GrandId.AspNetCore.Areas.GrandIdAuthentication.Controllers
{
    [Area(GrandIdAuthenticationConstants.AreaName)]
    [Route("/[area]/[action]")]
    public class GrandIdController : Controller
    {
        private readonly IAntiforgery _antiforgery;
        private readonly IGrandIdUserMessageLocalizer _bankIdUserMessageLocalizer;

        public GrandIdController(IAntiforgery antiforgery, IGrandIdUserMessageLocalizer bankIdUserMessageLocalizer)
        {
            _antiforgery = antiforgery;
            _bankIdUserMessageLocalizer = bankIdUserMessageLocalizer;
        }
    
        public ActionResult Login(string returnUrl)
        {
            if (!Url.IsLocalUrl(returnUrl))
            {
                throw new Exception(GrandIdAuthenticationConstants.InvalidReturnUrlErrorMessage);
            }

            var antiforgeryTokens = _antiforgery.GetAndStoreTokens(HttpContext);
            return View(new GrandIdLoginViewModel
            {
                ReturnUrl = returnUrl,
                AntiXsrfRequestToken = antiforgeryTokens.RequestToken,
                LoginScriptOptions = new GrandIdLoginScriptOptions()
                {
                    RefreshIntervalMs = GrandIdAuthenticationDefaults.StatusRefreshIntervalMs,

                    InitialStatusMessage = _bankIdUserMessageLocalizer.GetLocalizedString(MessageShortName.RFA1),
                    UnknownErrorMessage = _bankIdUserMessageLocalizer.GetLocalizedString(MessageShortName.RFA22),

                    GrandIdInitializeApiUrl = Url.Action(nameof(GrandIdApiController.InitializeAsync), "GrandIdApi"),
                    GrandIdStatusApiUrl = Url.Action(nameof(GrandIdApiController.StatusAsync), "GrandIdApi")
                }
            });
        }
    }
}