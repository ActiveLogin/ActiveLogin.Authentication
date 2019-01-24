namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.BankIdAuthentication.Models
{
    public class BankIdLoginApiInitializeResponse
    {
        private BankIdLoginApiInitializeResponse(bool isAutoLaunch, bool checkStatus, string orderRef, string redirectUri)
        {
            IsAutoLaunch = isAutoLaunch;
            CheckStatus = checkStatus;
            OrderRef = orderRef;
            RedirectUri = redirectUri;
        }


        public bool IsAutoLaunch { get; }
        public bool CheckStatus { get; }
        public string OrderRef { get; }
        public string RedirectUri { get; }


        public static BankIdLoginApiInitializeResponse AutoLaunch(string orderRef, string redirectUri)
        {
            return new BankIdLoginApiInitializeResponse(true, false, orderRef, redirectUri);
        }

        public static BankIdLoginApiInitializeResponse AutoLaunchAndCheckStatus(string orderRef, string redirectUri)
        {
            return new BankIdLoginApiInitializeResponse(true, true, orderRef, redirectUri);
        }

        public static BankIdLoginApiInitializeResponse ManualLaunch(string orderRef)
        {
            return new BankIdLoginApiInitializeResponse(false, true, orderRef, null);
        }
    }
}