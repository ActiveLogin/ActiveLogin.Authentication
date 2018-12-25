namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    public class BankIdSessionStateResponse : SessionStateResponseBase
    {
        internal BankIdSessionStateResponse(BankIdSessionStateFullResponse fullResponse)
        : base(fullResponse)
        {
            UserAttributes = fullResponse.UserAttributes;
        }

        internal BankIdSessionStateResponse(string sessionId, string username, BankIdSessionStateUserAttributes userAttributes)
            : base(sessionId, username)
        {
            UserAttributes = userAttributes;
        }

        public BankIdSessionStateUserAttributes UserAttributes { get; }
    }
}