namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.BankIdAuthentication.Models
{
    public class BankIdLoginApiInitializeResponse
    {
        internal BankIdLoginApiInitializeResponse(
            bool isAutoLaunch,
            bool showLaunchButton,
            bool checkStatus,
            string orderRef,
            string? redirectUri,
            string? qrCodeAsBase64)
        {
            IsAutoLaunch = isAutoLaunch;
            ShowLaunchButton = showLaunchButton;
            CheckStatus = checkStatus;
            OrderRef = orderRef;
            RedirectUri = redirectUri;
            QrCodeAsBase64 = qrCodeAsBase64;
        }


        public bool IsAutoLaunch { get; }
        public bool ShowLaunchButton { get; }
        public bool CheckStatus { get; }
        public string OrderRef { get; }
        public string? RedirectUri { get; }
        public string? QrCodeAsBase64 { get; set; }


        public static BankIdLoginApiInitializeResponse AutoLaunch(string orderRef, string redirectUri, bool showLaunchButton)
        {
            return new BankIdLoginApiInitializeResponse(true, showLaunchButton, false, orderRef, redirectUri, null);
        }

        public static BankIdLoginApiInitializeResponse AutoLaunchAndCheckStatus(string orderRef, string redirectUri, bool showLaunchButton)
        {
            return new BankIdLoginApiInitializeResponse(true, showLaunchButton, true, orderRef, redirectUri, null);
        }

        public static BankIdLoginApiInitializeResponse ManualLaunch(string orderRef, string qrCodeAsBase64)
        {
            return new BankIdLoginApiInitializeResponse(false, false, true, orderRef, null, qrCodeAsBase64);
        }

        public static BankIdLoginApiInitializeResponse ManualLaunch(string orderRef)
        {
            return new BankIdLoginApiInitializeResponse(false, false, true, orderRef, null, null);
        }
    }
}
