namespace ActiveLogin.Authentication.BankId.AspNetCore.Models
{
    public class BankIdLoginApiErrorResponse
    {
        public BankIdLoginApiErrorResponse(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public string ErrorMessage { get; set; }
    }
}