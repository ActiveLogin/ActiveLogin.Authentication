namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    public class FederatedLoginResponse : FederatedLoginResponseBase<DirectFederatedLoginFullResponse>
    {
        public FederatedLoginResponse()
        {

        }

        public FederatedLoginResponse(DirectFederatedLoginFullResponse fullResponse)
            : base(fullResponse)
        {
        }
    }
}