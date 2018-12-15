
using ActiveLogin.Authentication.BankId.AspNetCore.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.BankIdAuthentication.Models
{
    public class BankIdLoginViewModel
    {
        public string ReturnUrl { get; set; }

        public bool AutoLogin { get; set; }
        public string PersonalIdentityNumber { get; set; }
        public string OrderRef { get; set; }

        public string LoginOptions { get; set; }
        public BankIdLoginOptions UnprotectedLoginOptions { get; set; }

        public BankIdLoginScriptOptions LoginScriptOptions { get; set; }
        public string LoginScriptOptionsJson { get; set; }
        public string AntiXsrfRequestToken { get; set; }
    }
}