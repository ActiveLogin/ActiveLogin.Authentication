namespace ActiveLogin.Authentication.BankId.Api.Models;

public class SignResponse(string orderRef, string autoStartToken, string qrStartToken, string qrStartSecret) : Response(orderRef, autoStartToken, qrStartToken, qrStartSecret)
{
}
