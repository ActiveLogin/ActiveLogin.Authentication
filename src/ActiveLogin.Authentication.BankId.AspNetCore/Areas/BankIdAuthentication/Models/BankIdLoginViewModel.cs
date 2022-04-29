
using ActiveLogin.Authentication.BankId.AspNetCore.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.BankIdAuthentication.Models
{
    public class BankIdLoginViewModel
    {
        internal BankIdLoginViewModel(string returnUrl, string cancelReturnUrl, string loginOptions, BankIdLoginOptions unprotectedLoginOptions, BankIdLoginScriptOptions loginScriptOptions, string loginScriptOptionsJson, string antiXsrfRequestToken)
        {
            ReturnUrl = returnUrl;
            CancelReturnUrl = cancelReturnUrl;
            LoginOptions = loginOptions;
            UnprotectedLoginOptions = unprotectedLoginOptions;
            LoginScriptOptions = loginScriptOptions;
            LoginScriptOptionsJson = loginScriptOptionsJson;
            AntiXsrfRequestToken = antiXsrfRequestToken;
        }

        public string ReturnUrl { get; }
        public string CancelReturnUrl { get; }

        public string LoginOptions { get; }
        public BankIdLoginOptions UnprotectedLoginOptions { get; }

        public BankIdLoginScriptOptions LoginScriptOptions { get; }
        public string LoginScriptOptionsJson { get; }
        public string AntiXsrfRequestToken { get; }
    }
}
