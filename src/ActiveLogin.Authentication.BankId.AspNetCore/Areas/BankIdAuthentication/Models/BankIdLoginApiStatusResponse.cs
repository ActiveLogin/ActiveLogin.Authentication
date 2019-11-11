namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.BankIdAuthentication.Models
{
    public class BankIdLoginApiStatusResponse
    {
        internal BankIdLoginApiStatusResponse(string statusMessage, bool retryLogin)
        {
            StatusMessage = statusMessage;
            RetryLogin = retryLogin;
        }

        internal BankIdLoginApiStatusResponse(bool isFinished, string statusMessage, string redirectUri)
        {
            IsFinished = isFinished;
            StatusMessage = statusMessage;
            RedirectUri = redirectUri;
        }

        public bool IsFinished { get; }
        public string StatusMessage { get; }
        public string RedirectUri { get; }
        public bool RetryLogin { get; }


        public static BankIdLoginApiStatusResponse Finished(string redirectUri)
        {
            return new BankIdLoginApiStatusResponse(true, null, redirectUri);
        }

        public static BankIdLoginApiStatusResponse Pending(string statusMessage)
        {
            return new BankIdLoginApiStatusResponse(false, statusMessage, null);
        }

        public static BankIdLoginApiStatusResponse Retry(string statusMessage)
        {
            return new BankIdLoginApiStatusResponse(statusMessage, true);
        }
    }
}
