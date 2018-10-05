namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    public class FederatedLoginRequest
    {
        public FederatedLoginRequest(string authenticateServiceKey, string callbackUrl)
            : this(authenticateServiceKey, callbackUrl, null)
        {
        }

        public FederatedLoginRequest(string authenticateServiceKey, string callbackUrl, string personalIdentityNumber)
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