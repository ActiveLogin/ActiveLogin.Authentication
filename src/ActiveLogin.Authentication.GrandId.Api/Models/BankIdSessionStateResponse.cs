namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    public class BankIdSessionStateResponse : SessionStateResponseBase<BankIdSessionStateFullResponse, BankIdSessionStateUserAttributes>
    {
        public BankIdSessionStateResponse()
        {

        }

        public BankIdSessionStateResponse(BankIdSessionStateFullResponse fullResponse)
        : base(fullResponse)
        {
        }
    }
}