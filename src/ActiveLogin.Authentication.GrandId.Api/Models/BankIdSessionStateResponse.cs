namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    public class BankIdSessionStateResponse : SessionStateResponseBase
    {
        public BankIdSessionStateResponse()
        {

        }

        internal BankIdSessionStateResponse(BankIdSessionStateFullResponse fullResponse)
        : base(fullResponse)
        {
            UserAttributes = fullResponse.UserAttributes;
        }

        public BankIdSessionStateUserAttributes UserAttributes { get; set; }
    }
}