namespace ActiveLogin.Authentication.BankId.Api.Models;

public class SignResponse : Response
{
    public SignResponse(string orderRef, string autoStartToken, string qrStartToken, string qrStartSecret)
        : base(orderRef, autoStartToken, qrStartToken, qrStartSecret)
    {
    }
}
