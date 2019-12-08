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
    }
}
