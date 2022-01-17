namespace ActiveLogin.Authentication.BankId.AspNetCore.StateHandling
{
    public class BankIdInvalidStateContext
    {  
        internal BankIdInvalidStateContext(string cancelReturnUrl) 
        {
            CancelReturnUrl = cancelReturnUrl;
        }

        public string CancelReturnUrl { get; }
    }
}
