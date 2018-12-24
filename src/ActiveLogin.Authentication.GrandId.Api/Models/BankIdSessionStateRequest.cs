namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    public class BankIdSessionStateRequest
    {
        public BankIdSessionStateRequest(string authenticateServiceKey, string sessionId)
        {
            AuthenticateServiceKey = authenticateServiceKey;
            SessionId = sessionId;
        }

        public string AuthenticateServiceKey { get; }
        public string SessionId { get; }
    }
}