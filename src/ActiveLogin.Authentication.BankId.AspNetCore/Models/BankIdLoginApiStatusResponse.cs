namespace ActiveLogin.Authentication.BankId.AspNetCore.Models
{
    public class BankIdLoginApiStatusResponse
    {
        public bool IsFinished { get; set; }

        public string StatusMessage { get; set; }
        public string RedirectUri { get; set; }

        public static BankIdLoginApiStatusResponse Finished(string redirectUri)
        {
            return new BankIdLoginApiStatusResponse
            {
                IsFinished = true,
                RedirectUri = redirectUri
            };
        }

        public static BankIdLoginApiStatusResponse Pending(string statusMessage)
        {
            return new BankIdLoginApiStatusResponse
            {
                IsFinished = false,

                StatusMessage = statusMessage
            };
        }
    }
}