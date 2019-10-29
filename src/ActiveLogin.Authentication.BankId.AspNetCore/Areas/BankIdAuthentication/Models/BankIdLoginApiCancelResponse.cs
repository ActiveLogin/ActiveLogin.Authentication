namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.BankIdAuthentication.Models
{
    public class BankIdLoginApiCancelResponse
    {
        public string CancelReturnUrl { get; set; }

        internal BankIdLoginApiCancelResponse()
        {

        }

        public static BankIdLoginApiCancelResponse Cancelled(string cancelReturnUrl)
        {
            return new BankIdLoginApiCancelResponse { CancelReturnUrl = cancelReturnUrl };
        }
    }
}
