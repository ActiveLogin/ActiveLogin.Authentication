
using ActiveLogin.Authentication.BankId.AspNetCore.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.BankIdAuthentication.Models
{
    public class BankIdLoginViewModel
    {
        internal BankIdLoginViewModel(string returnUrl, string cancelReturnUrl, bool autoLogin, string personalIdentityNumber, string loginOptions, BankIdLoginOptions unprotectedLoginOptions, BankIdLoginScriptOptions loginScriptOptions, string loginScriptOptionsJson, string antiXsrfRequestToken)
        {
            ReturnUrl = returnUrl;
            CancelReturnUrl = cancelReturnUrl;
            AutoLogin = autoLogin;
            PersonalIdentityNumber = personalIdentityNumber;
            LoginOptions = loginOptions;
            UnprotectedLoginOptions = unprotectedLoginOptions;
            LoginScriptOptions = loginScriptOptions;
            LoginScriptOptionsJson = loginScriptOptionsJson;
            AntiXsrfRequestToken = antiXsrfRequestToken;
        }

        public string ReturnUrl { get; set; }
        public string CancelReturnUrl { get; set; }

        public bool AutoLogin { get; set; }
        public string PersonalIdentityNumber { get; set; }

        public string LoginOptions { get; set; }
        public BankIdLoginOptions UnprotectedLoginOptions { get; set; }

        public BankIdLoginScriptOptions LoginScriptOptions { get; set; }
        public string LoginScriptOptionsJson { get; set; }
        public string AntiXsrfRequestToken { get; set; }
    }
}
