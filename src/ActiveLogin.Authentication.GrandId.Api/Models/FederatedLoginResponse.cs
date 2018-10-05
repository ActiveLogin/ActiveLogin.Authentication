namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    public class FederatedLoginResponse
    {
        public FederatedLoginResponse()
        {
            
        }

        public FederatedLoginResponse(FederatedLoginFullResponse fullResponse)
        {
            RedirectUrl = fullResponse.RedirectUrl;
            SessionId = fullResponse.SessionId;
        }

        public string RedirectUrl { get; set; }
        public string SessionId { get; set; }
    }
}
