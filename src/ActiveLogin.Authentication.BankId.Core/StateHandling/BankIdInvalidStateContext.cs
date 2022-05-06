namespace ActiveLogin.Authentication.BankId.Core.StateHandling;

public class BankIdInvalidStateContext
{
    public BankIdInvalidStateContext(string cancelReturnUrl) 
    {
        CancelReturnUrl = cancelReturnUrl;
    }

    public string CancelReturnUrl { get; }
}
