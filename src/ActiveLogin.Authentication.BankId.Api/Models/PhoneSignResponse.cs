namespace ActiveLogin.Authentication.BankId.Api.Models;

public class PhoneSignResponse : PhoneResponse
{
    public PhoneSignResponse(string orderRef)
        : base(orderRef)
    {
    }
}
