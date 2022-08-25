namespace ActiveLogin.Authentication.GrandId.Api.Models;

public class BankIdFederatedLoginResponse : FederatedLoginResponseBase
{
    internal BankIdFederatedLoginResponse(BankIdFederatedLoginFullResponse fullResponse)
        : base(fullResponse)
    {
    }

    public BankIdFederatedLoginResponse(string sessionId, string redirectUrl)
        : base(sessionId, redirectUrl)
    {
    }
}