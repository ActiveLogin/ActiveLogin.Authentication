namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    public class BankIdFederatedLoginResponse : FederatedLoginResponseBase<BankIdFederatedLoginFullResponse>
    {
        public BankIdFederatedLoginResponse()
        {

        }

        public BankIdFederatedLoginResponse(BankIdFederatedLoginFullResponse fullResponse)
        : base(fullResponse)
        {
        }
    }
}
