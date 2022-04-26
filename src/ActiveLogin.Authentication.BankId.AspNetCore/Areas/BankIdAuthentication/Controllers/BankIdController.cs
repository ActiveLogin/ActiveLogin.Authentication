using System;
using System.Threading.Tasks;
using ActiveLogin.Authentication.BankId.Api.UserMessage;
using ActiveLogin.Authentication.BankId.AspNetCore.Areas.BankIdAuthentication.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.StateHandling;
using ActiveLogin.Authentication.BankId.AspNetCore.UserMessage;
using ActiveLogin.Authentication.BankId.AspNetCore.Serialization;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.BankIdAuthentication.Controllers
{
    [Area(BankIdConstants.AreaName)]
    [Route("/[area]/[action]")]
    [AllowAnonymous]
    public class BankIdController : Controller
    {
        private readonly IAntiforgery _antiforgery;
        private readonly IBankIdUserMessageLocalizer _bankIdUserMessageLocalizer;
        private readonly IBankIdLoginOptionsProtector _loginOptionsProtector;
        private readonly IStringLocalizer<BankIdHandler> _localizer;
        private readonly IBankIdInvalidStateHandler _bankIdInvalidStateHandler;

        public BankIdController(
            IAntiforgery antiforgery,
            IBankIdUserMessageLocalizer bankIdUserMessageLocalizer,
            IBankIdLoginOptionsProtector loginOptionsProtector,
            IStringLocalizer<BankIdHandler> localizer,
            IBankIdInvalidStateHandler bankIdInvalidStateHandler)
        {
            _antiforgery = antiforgery;
            _bankIdUserMessageLocalizer = bankIdUserMessageLocalizer;
            _loginOptionsProtector = loginOptionsProtector;
            _localizer = localizer;
            _bankIdInvalidStateHandler = bankIdInvalidStateHandler;
        }

        [HttpGet]
        public async Task<ActionResult> Login(string returnUrl, string loginOptions)
        {
            if (!Url.IsLocalUrl(returnUrl))
            {
                throw new Exception(BankIdConstants.InvalidReturnUrlErrorMessage);
            }

            var unprotectedLoginOptions = _loginOptionsProtector.Unprotect(loginOptions);
            if (!HasStateCookie(unprotectedLoginOptions))
            {
                var invalidStateContext = new BankIdInvalidStateContext(unprotectedLoginOptions.CancelReturnUrl);
                await _bankIdInvalidStateHandler.HandleAsync(HttpContext, invalidStateContext);

                return new EmptyResult();
            }

            var antiforgeryTokens = _antiforgery.GetAndStoreTokens(HttpContext);
            var viewModel = GetLoginViewModel(returnUrl, loginOptions, unprotectedLoginOptions, antiforgeryTokens);
            return View(viewModel);
        }

        private bool HasStateCookie(BankIdLoginOptions loginOptions)
        {
            if (string.IsNullOrEmpty(loginOptions.StateCookieName)
               || !HttpContext.Request.Cookies.ContainsKey(loginOptions.StateCookieName))
            {
                return false;
            }

            return !string.IsNullOrEmpty(HttpContext.Request.Cookies[loginOptions.StateCookieName]);
        }

        private BankIdLoginViewModel GetLoginViewModel(string returnUrl, string loginOptions, BankIdLoginOptions unprotectedLoginOptions, AntiforgeryTokenSet antiforgeryTokens)
        {
            var initialStatusMessage = GetInitialStatusMessage(unprotectedLoginOptions);
            var loginScriptOptions = new BankIdLoginScriptOptions(
                Url.Action("Initialize", "BankIdApi") ?? throw new Exception("Could not get URL for BankIdApi.Initialize"),
                Url.Action("Status", "BankIdApi") ?? throw new Exception("Could not get URL for BankIdApi.Status"),
                Url.Action("QrCode", "BankIdApi") ?? throw new Exception("Could not get URL for BankIdApi.QrCode"),
                Url.Action("Cancel", "BankIdApi") ?? throw new Exception("Could not get URL for BankIdApi.Cancel")
                )
            {
                StatusRefreshIntervalMs = BankIdDefaults.StatusRefreshIntervalMs,
                QrCodeRefreshIntervalMs = BankIdDefaults.QrCodeRefreshIntervalMs,

                InitialStatusMessage = _bankIdUserMessageLocalizer.GetLocalizedString(initialStatusMessage),
                UnknownErrorMessage = _bankIdUserMessageLocalizer.GetLocalizedString(MessageShortName.RFA22),

                UnsupportedBrowserErrorMessage = _localizer["UnsupportedBrowser_ErrorMessage"]
            };

            return new BankIdLoginViewModel(
                returnUrl,
                Url.Content(unprotectedLoginOptions.CancelReturnUrl),
                unprotectedLoginOptions.IsAutoLogin(),
                unprotectedLoginOptions.PersonalIdentityNumber?.To12DigitString() ?? string.Empty,
                loginOptions,
                unprotectedLoginOptions,
                loginScriptOptions,
                SystemTextJsonSerializer.Serialize(loginScriptOptions),
                antiforgeryTokens.RequestToken ?? throw new ArgumentNullException(nameof(antiforgeryTokens.RequestToken))
            );
        }

        private static MessageShortName GetInitialStatusMessage(BankIdLoginOptions loginOptions)
        {
            if (loginOptions.SameDevice)
            {
                return MessageShortName.RFA13;
            }

            if (loginOptions.UseQrCode)
            {
                return MessageShortName.RFA1QR;
            }

            return MessageShortName.RFA1;
        }
    }
}
