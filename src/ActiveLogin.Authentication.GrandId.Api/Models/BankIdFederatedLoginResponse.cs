namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    public class BankIdFederatedLoginResponse : FederatedLoginResponseBase
    {
        public BankIdFederatedLoginResponse()
        {

        }

        internal BankIdFederatedLoginResponse(BankIdFederatedLoginFullResponse fullResponse)
        : base(fullResponse)
        {
        }
    }
}
