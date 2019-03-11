namespace ActiveLogin.Authentication.BankId.AspNetCore.Models
{
    public class BankIdOrderRef
    {
        public BankIdOrderRef(string orderRef)
        {
            OrderRef = orderRef;
        }

        public string OrderRef { get; }
    }
}
