namespace ActiveLogin.Authentication.BankId.AspNetCore.Models
{
    public class BankIdLoginApiInitializeResponse
    {
        public BankIdLoginApiInitializeResponse(string orderRef)
        {
            OrderRef = orderRef;
        }

        public string OrderRef { get; }
    }
}