namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    public class SessionStateRequest
    {
        public SessionStateRequest(string authenticateServiceKey, string sessionId)
        {
            AuthenticateServiceKey = authenticateServiceKey;
            SessionId = sessionId;
        }

        public string AuthenticateServiceKey { get; set; }
        public string SessionId { get; set; }
    }
}