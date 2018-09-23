namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    public class AuthRequest
    {
        public AuthRequest(string authenticateServiceKey, string callbackUrl)
        {
            AuthenticateServiceKey = authenticateServiceKey;
            CallbackUrl = callbackUrl;
        }

        public string AuthenticateServiceKey { get; set; }
        public string CallbackUrl { get; set; }
    }
}