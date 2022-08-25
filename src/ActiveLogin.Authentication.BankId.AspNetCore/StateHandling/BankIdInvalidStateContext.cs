namespace ActiveLogin.Authentication.BankId.AspNetCore.StateHandling;

public class BankIdInvalidStateContext
{
    public BankIdInvalidStateContext(string cancelReturnUrl) 
    {
        CancelReturnUrl = cancelReturnUrl;
    }

    public string CancelReturnUrl { get; }
}
