namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    public class FederatedDirectLoginResponse
    {
        public FederatedDirectLoginResponse()
        {

        }

        internal FederatedDirectLoginResponse(FederatedDirectLoginFullResponse fullResponse)
        {
            SessionId = fullResponse.SessionId;
            Username = fullResponse.Username;
            UserAttributes = fullResponse.UserAttributes;
        }

        public string SessionId { get; set; }

        public string Username { get; set; }

        public FederatedDirectLoginUserAttributes UserAttributes { get; set; }
    }
}
