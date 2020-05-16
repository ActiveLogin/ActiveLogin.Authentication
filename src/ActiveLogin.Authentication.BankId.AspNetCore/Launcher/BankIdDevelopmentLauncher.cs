using ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Launcher
{
    internal class BankIdDevelopmentLauncher : IBankIdLauncher
    {
        public string GetLaunchUrl(BankIdSupportedDevice device, LaunchUrlRequest request)
        {
            if (device.DeviceOs == BankIdSupportedDeviceOs.Ios)
            {
                return request.RedirectUrl;
            }

            return "#";
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
