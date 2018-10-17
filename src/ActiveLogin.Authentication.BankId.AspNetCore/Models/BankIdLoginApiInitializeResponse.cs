namespace ActiveLogin.Authentication.BankId.AspNetCore.Models
{
    public class BankIdLoginApiInitializeResponse
    {
        private BankIdLoginApiInitializeResponse(bool isAutoLaunch, string orderRef, string redirectUri)
        {
            IsAutoLaunch = isAutoLaunch;
            OrderRef = orderRef;
            RedirectUri = redirectUri;
        }


        public bool IsAutoLaunch { get; }
        public string OrderRef { get; }
        public string RedirectUri { get; }


        public static BankIdLoginApiInitializeResponse AutoLaunch(string redirectUri)
        {
            return new BankIdLoginApiInitializeResponse(true, null, redirectUri);
        }

        public static BankIdLoginApiInitializeResponse ManualLaunch(string orderRef)
        {
            return new BankIdLoginApiInitializeResponse(false, orderRef, null);
        }
    }
}