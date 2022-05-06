namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;

public class BankIdLoginApiCancelResponse
{
    internal BankIdLoginApiCancelResponse()
    {
    }

    public static BankIdLoginApiCancelResponse Cancelled()
    {
        return new BankIdLoginApiCancelResponse();
    }
}
