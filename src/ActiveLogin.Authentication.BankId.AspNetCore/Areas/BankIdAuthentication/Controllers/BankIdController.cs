using System;
using ActiveLogin.Authentication.BankId.Api.UserMessage;
using ActiveLogin.Authentication.BankId.AspNetCore.Areas.BankIdAuthentication.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.UserMessage;
using ActiveLogin.Authentication.Common.Serialization;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.BankIdAuthentication.Controllers
{
    [Area(BankIdAuthenticationConstants.AreaName)]
    [Route("/[area]/[action]")]
    [AllowAnonymous]
    public class BankIdController : Controller
    {
        private readonly IAntiforgery _antiforgery;
        private readonly IBankIdUserMessageLocalizer _bankIdUserMessageLocalizer;
        private readonly IBankIdLoginOptionsProtector _loginOptionsProtector;
        private readonly IStringLocalizer<BankIdAuthenticationHandler> _localizer;

        public BankIdController(
            IAntiforgery antiforgery,
            IBankIdUserMessageLocalizer bankIdUserMessageLocalizer,
            IBankIdLoginOptionsProtector loginOptionsProtector,
            IStringLocalizer<BankIdAuthenticationHandler> localizer)
        {
            _antiforgery = antiforgery;
            _bankIdUserMessageLocalizer = bankIdUserMessageLocalizer;
            _loginOptionsProtector = loginOptionsProtector;
            _localizer = localizer;
        }
    
        public ActionResult Login(string returnUrl, string loginOptions)
        {
            if (!Url.IsLocalUrl(returnUrl))
            {
                throw new Exception(BankIdAuthenticationConstants.InvalidReturnUrlErrorMessage);
            }

            var unprotectedLoginOptions = _loginOptionsProtector.Unprotect(loginOptions);
            var antiforgeryTokens = _antiforgery.GetAndStoreTokens(HttpContext);

            var viewModel = GetLoginViewModel(returnUrl, loginOptions, unprotectedLoginOptions, antiforgeryTokens);
            return View(viewModel);
        }
        
        private BankIdLoginViewModel GetLoginViewModel(string returnUrl, string loginOptions, BankIdLoginOptions unprotectedLoginOptions, AntiforgeryTokenSet antiforgeryTokens)
        {
            var loginScriptOptions = new BankIdLoginScriptOptions
            {
                RefreshIntervalMs = BankIdAuthenticationDefaults.StatusRefreshIntervalMs,

                InitialStatusMessage = _bankIdUserMessageLocalizer.GetLocalizedString(MessageShortName.RFA13),
                UnknownErrorMessage = _bankIdUserMessageLocalizer.GetLocalizedString(MessageShortName.RFA22),

                UnsuportedBrowserErrorMessage = _localizer["UnsuportedBrowser_ErrorMessage"],

                BankIdInitializeApiUrl = Url.Action(nameof(BankIdApiController.InitializeAsync), "BankIdApi"),
                BankIdStatusApiUrl = Url.Action(nameof(BankIdApiController.StatusAsync), "BankIdApi")
            };

            return new BankIdLoginViewModel
            {
                ReturnUrl = returnUrl,

                AutoLogin = unprotectedLoginOptions.IsAutoLogin(),
                PersonalIdentityNumber = unprotectedLoginOptions.PersonalIdentityNumber?.To12DigitString() ?? string.Empty,

                LoginOptions = loginOptions,
                UnprotectedLoginOptions = unprotectedLoginOptions,

                AntiXsrfRequestToken = antiforgeryTokens.RequestToken,
                LoginScriptOptions = loginScriptOptions,
                LoginScriptOptionsJson = SystemRuntimeJsonSerializer.Serialize(loginScriptOptions)
            };
        }
    }
}