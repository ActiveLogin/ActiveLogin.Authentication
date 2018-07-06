namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.BankIdAuthentication.Models
{
    public class BankIdLoginViewModel
    {
        public string ReturnUrl { get; set; }
        public BankIdLoginScriptOptions LoginScriptOptions { get; set; }
        public string AntiXsrfRequestToken { get; set; }
    }
}