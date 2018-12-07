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
        }

        public string SessionId { get; set; }
    }
}