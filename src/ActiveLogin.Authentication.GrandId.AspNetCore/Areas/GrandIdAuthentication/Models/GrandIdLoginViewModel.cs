namespace ActiveLogin.Authentication.GrandId.AspNetCore.Areas.GrandIdAuthentication.Models
{
    public class GrandIdLoginViewModel
    {
        public string ReturnUrl { get; set; }
        public GrandIdLoginScriptOptions LoginScriptOptions { get; set; }
        public string AntiXsrfRequestToken { get; set; }
    }
}