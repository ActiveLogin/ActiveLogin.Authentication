namespace ActiveLogin.Authentication.BankId.AspNetCore.Launcher
{
    public class LaunchUrlRequest
    {
        public LaunchUrlRequest(string redirectUrl, string autoStartToken)
        {
            RedirectUrl = redirectUrl;
            AutoStartToken = autoStartToken;
            RelyingPartyReference = null;
        }

        public LaunchUrlRequest(string redirectUrl, string autoStartToken, string relyingPartyReference)
        {
            RedirectUrl = redirectUrl;
            AutoStartToken = autoStartToken;
            RelyingPartyReference = relyingPartyReference;
        }

        public string RedirectUrl { get; }
        public string AutoStartToken { get; }
        public string? RelyingPartyReference { get; }
    }
}
