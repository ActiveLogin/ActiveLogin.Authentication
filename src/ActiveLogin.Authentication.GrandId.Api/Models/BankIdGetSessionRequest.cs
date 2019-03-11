namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    public class BankIdGetSessionRequest
    {
        public BankIdGetSessionRequest(string sessionId)
        {
            SessionId = sessionId;
        }

        public string SessionId { get; }
    }
}
