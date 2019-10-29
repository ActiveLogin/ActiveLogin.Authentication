namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.BankIdAuthentication.Models
{
    public class BankIdLoginApiCancelResponse
    {
        internal BankIdLoginApiCancelResponse()
        {

        }

        public static BankIdLoginApiCancelResponse Cancelled()
        {
            return new BankIdLoginApiCancelResponse();
        }
    }
}
