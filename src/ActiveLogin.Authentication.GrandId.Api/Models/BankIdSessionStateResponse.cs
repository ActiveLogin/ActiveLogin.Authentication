namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    public class BankIdSessionStateResponse : SessionStateResponseBase<BankIdSessionStateFullResponse, SessionUserAttributes>
    {
        public BankIdSessionStateResponse()
        {

        }

        public BankIdSessionStateResponse(BankIdSessionStateFullResponse fullResponse)
        : base(fullResponse)
        {
            SessionId = fullResponse.SessionId;
            UserName = fullResponse.UserName;
            UserAttributes = fullResponse.UserAttributes;
        }
    }
}