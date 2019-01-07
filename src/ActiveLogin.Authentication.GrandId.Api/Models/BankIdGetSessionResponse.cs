namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    public class BankIdGetSessionResponse : GetSessionResponseBase
    {
        internal BankIdGetSessionResponse(BankIdGetSessionFullResponse fullResponse)
        : base(fullResponse)
        {
            UserAttributes = fullResponse.UserAttributes;
        }

        internal BankIdGetSessionResponse(string sessionId, string username, BankIdGetSessionUserAttributes userAttributes)
            : base(sessionId, username)
        {
            UserAttributes = userAttributes;
        }

        public BankIdGetSessionUserAttributes UserAttributes { get; }
    }
}