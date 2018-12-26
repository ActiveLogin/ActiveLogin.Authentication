namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    public class BankIdGetSessionResponse : SessionStateResponseBase
    {
        internal BankIdGetSessionResponse(BankIdSessionStateFullResponse fullResponse)
        : base(fullResponse)
        {
            UserAttributes = fullResponse.UserAttributes;
        }

        internal BankIdGetSessionResponse(string sessionId, string username, BankIdSessionStateUserAttributes userAttributes)
            : base(sessionId, username)
        {
            UserAttributes = userAttributes;
        }

        public BankIdSessionStateUserAttributes UserAttributes { get; }
    }
}