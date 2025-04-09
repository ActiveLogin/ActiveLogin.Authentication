namespace ActiveLogin.Authentication.BankId.Api.Models;

public class AuthResponse(string orderRef, string autoStartToken, string qrStartToken, string qrStartSecret) : Response(orderRef, autoStartToken, qrStartToken, qrStartSecret)
{
}
