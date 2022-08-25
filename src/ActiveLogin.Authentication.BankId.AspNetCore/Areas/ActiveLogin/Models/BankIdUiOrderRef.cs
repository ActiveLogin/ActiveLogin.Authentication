namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;

public class BankIdUiOrderRef
{
    public BankIdUiOrderRef(string orderRef)
    {
        OrderRef = orderRef;
    }

    public string OrderRef { get; }
}
