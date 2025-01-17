namespace ActiveLogin.Authentication.BankId.Api.Models;

public class PaymentResponse(string orderRef, string autoStartToken, string qrStartToken, string qrStartSecret) : Response(orderRef, autoStartToken, qrStartToken, qrStartSecret)
{
}
