using ActiveLogin.Authentication.BankId.Api.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.BankIdAuthentication.Models
{
    public class BankIdLoginApiErrorResponse
    {
        internal BankIdLoginApiErrorResponse(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public BankIdLoginApiErrorResponse(string errorMessage, CollectHintCode hintCode)
        {
            ErrorMessage = errorMessage;
            HintCode = hintCode.ToString();
        }

        public string ErrorMessage { get; }

        public string HintCode { get; }
    }
}
