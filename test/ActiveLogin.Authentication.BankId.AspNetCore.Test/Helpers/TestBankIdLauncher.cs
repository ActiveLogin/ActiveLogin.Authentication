using ActiveLogin.Authentication.BankId.AspNetCore.Launcher;
using ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Test.Helpers
{
    internal class TestBankIdLauncher : IBankIdLauncher
    {
        public string GetLaunchUrl(BankIdSupportedDevice device, LaunchUrlRequest request)
        {
            return request.RedirectUrl;
        }

        public bool GetDeviceMightRequireUserInteractionToLaunchBankIdApp(BankIdSupportedDevice detectedDevice)
        {
            return false;
        }

        public bool GetDeviceWillReloadPageOnReturnFromBankIdApp(BankIdSupportedDevice detectedDevice)
        {
            return false;
        }
    }
}
