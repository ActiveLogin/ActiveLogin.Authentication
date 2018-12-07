namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    public abstract class FederatedLoginResponseBase<TFullResponse> where TFullResponse : FederatedLoginFullResponseBase
    {
        protected FederatedLoginResponseBase()
        {
            
        }

        protected FederatedLoginResponseBase(TFullResponse fullResponse)
        {
            SessionId = fullResponse.SessionId;
            RedirectUrl = fullResponse.RedirectUrl;
        }

        public string SessionId { get; set; }

        public string RedirectUrl { get; set; }
    }
}