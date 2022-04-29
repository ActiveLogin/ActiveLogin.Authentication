namespace ActiveLogin.Authentication.BankId.Core.StateHandling;

public class BankIdInvalidStateContext
{  
    internal BankIdInvalidStateContext(string cancelReturnUrl) 
    {
        CancelReturnUrl = cancelReturnUrl;
    }

    public string CancelReturnUrl { get; }
}
