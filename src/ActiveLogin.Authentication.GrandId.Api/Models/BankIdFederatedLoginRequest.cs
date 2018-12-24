namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    public class BankIdFederatedLoginRequest
    {
        public BankIdFederatedLoginRequest(string authenticateServiceKey, string callbackUrl)
            : this(authenticateServiceKey, callbackUrl, null)
        {
        }

        public BankIdFederatedLoginRequest(string authenticateServiceKey, string callbackUrl, string personalIdentityNumber)
        {
            AuthenticateServiceKey = authenticateServiceKey;
            CallbackUrl = callbackUrl;
            PersonalIdentityNumber = personalIdentityNumber;
        }

        public string AuthenticateServiceKey { get; }
        public string CallbackUrl { get; }

        /// <summary>
        /// When using the login method Bankid Another Device you may use this parameter in the format of 12 digits, YYYYMMDDnnnn, if you know the users personal number in advance.
        /// When using the pnr parameter the authentication must be completed on a mobile device, such as phone or tablet.
        /// </summary>
        public string PersonalIdentityNumber { get; }
    }
}