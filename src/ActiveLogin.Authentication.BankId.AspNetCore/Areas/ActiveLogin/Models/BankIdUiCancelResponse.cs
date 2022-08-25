namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;

public class BankIdUiCancelResponse
{
    internal BankIdUiCancelResponse()
    {
    }

    public static BankIdUiCancelResponse Cancelled()
    {
        return new BankIdUiCancelResponse();
    }
}
