namespace ActiveLogin.Authentication.BankId.Api.Models;

public class AuthResponse : Response
{
    public AuthResponse(string orderRef, string autoStartToken, string qrStartToken, string qrStartSecret)
        : base(orderRef, autoStartToken, qrStartToken, qrStartSecret)
    {
    }
}
