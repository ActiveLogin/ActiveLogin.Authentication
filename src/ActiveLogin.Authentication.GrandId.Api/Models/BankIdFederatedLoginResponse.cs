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
            RedirectUrl = fullResponse.RedirectUrl;
        }

        public string RedirectUrl { get; set; }
    }
}
