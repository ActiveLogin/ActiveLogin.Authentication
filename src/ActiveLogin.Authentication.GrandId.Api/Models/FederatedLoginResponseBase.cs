namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    public abstract class FederatedLoginResponseBase
    {
        protected FederatedLoginResponseBase()
        {
            
        }

        internal FederatedLoginResponseBase(FederatedLoginFullResponseBase fullResponse)
        {
            SessionId = fullResponse.SessionId;
            RedirectUrl = fullResponse.RedirectUrl;
        }

        public string SessionId { get; set; }

        public string RedirectUrl { get; set; }
    }
}