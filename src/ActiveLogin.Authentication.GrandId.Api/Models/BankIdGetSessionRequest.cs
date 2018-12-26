namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    public class BankIdGetSessionRequest
    {
        public BankIdGetSessionRequest(string authenticateServiceKey, string sessionId)
        {
            AuthenticateServiceKey = authenticateServiceKey;
            SessionId = sessionId;
        }

        public string AuthenticateServiceKey { get; }
        public string SessionId { get; }
    }
}