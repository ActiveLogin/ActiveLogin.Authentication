namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    public class AuthRequest
    {
        public AuthRequest(string authenticateServiceKey, string callbackUrl)
            : this(authenticateServiceKey, callbackUrl, null)
        {
        }

        public AuthRequest(string authenticateServiceKey, string callbackUrl, string personalIdentityNumber)
        {
            AuthenticateServiceKey = authenticateServiceKey;
            CallbackUrl = callbackUrl;
            PersonalIdentityNumber = personalIdentityNumber;
        }

        public string AuthenticateServiceKey { get; set; }
        public string CallbackUrl { get; set; }
        public string PersonalIdentityNumber { get; set; }
    }
}