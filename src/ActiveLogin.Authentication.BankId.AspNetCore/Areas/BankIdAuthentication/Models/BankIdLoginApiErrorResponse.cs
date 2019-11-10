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
            Retry = CollectHintCode.StartFailed.Equals(hintCode);
        }

        public string ErrorMessage { get; }

        public bool Retry { get; }
    }
}
