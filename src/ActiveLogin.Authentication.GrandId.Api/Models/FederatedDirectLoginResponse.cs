namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    public class FederatedDirectLoginResponse
    {
        internal FederatedDirectLoginResponse(FederatedDirectLoginFullResponse fullResponse)
        {
            SessionId = fullResponse.SessionId;
            Username = fullResponse.Username;
            UserAttributes = fullResponse.UserAttributes;
        }

        internal FederatedDirectLoginResponse(string sessionId, string username, FederatedDirectLoginUserAttributes userAttributes)
        {
            SessionId = sessionId;
            Username = username;
            UserAttributes = userAttributes;
        }

        public string SessionId { get; }

        public string Username { get; }

        public FederatedDirectLoginUserAttributes UserAttributes { get; }
    }
}
