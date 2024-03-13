namespace ActiveLogin.Authentication.BankId.Api.Models;

public class PhoneAuthResponse : PhoneResponse
{
    public PhoneAuthResponse(string orderRef)
        : base(orderRef)
    {
    }
}
