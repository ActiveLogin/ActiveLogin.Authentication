namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.BankIdAuthentication.Models
{
    public class BankIdLoginApiCancelResponse
    {
        internal BankIdLoginApiCancelResponse(string cancelReturnUrl)
        {
            CancelReturnUrl = cancelReturnUrl;
        }

        public static BankIdLoginApiCancelResponse Cancelled(string cancelReturnUrl)
        {
            return new BankIdLoginApiCancelResponse(cancelReturnUrl);
        }

        public string CancelReturnUrl { get; }
    }
}
