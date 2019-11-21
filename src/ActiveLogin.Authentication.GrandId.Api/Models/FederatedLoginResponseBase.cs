namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    public abstract class FederatedLoginResponseBase
    {
        private protected FederatedLoginResponseBase(FederatedLoginFullResponseBase fullResponse)
        {
            SessionId = fullResponse.SessionId;
            RedirectUrl = fullResponse.RedirectUrl;
        }

        private protected FederatedLoginResponseBase(string sessionId, string redirectUrl)
        {
            SessionId = sessionId;
            RedirectUrl = redirectUrl;
        }

        public string? SessionId { get; }

        public string? RedirectUrl { get; }
    }
}