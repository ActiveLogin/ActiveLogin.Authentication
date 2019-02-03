namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public class LaunchUrlRequest
    {
        internal LaunchUrlRequest(string redirectUrl, string autoStartToken)
            : this(redirectUrl, autoStartToken, null)
        {
        }

        internal LaunchUrlRequest(string redirectUrl, string autoStartToken, string relyingPartyReference)
        {
            RedirectUrl = redirectUrl;
            AutoStartToken = autoStartToken;
            RelyingPartyReference = relyingPartyReference;
        }

        public string RedirectUrl { get; }
        public string AutoStartToken { get; }
        public string RelyingPartyReference { get; }
    }
}