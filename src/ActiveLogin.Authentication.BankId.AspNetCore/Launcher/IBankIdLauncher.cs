using ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Launcher
{
    public interface IBankIdLauncher
    {
        string GetLaunchUrl(BankIdSupportedDevice device, LaunchUrlRequest request);
        bool GetDeviceMightRequireUserInteractionToLaunchBankIdApp(BankIdSupportedDevice detectedDevice);
        bool GetDeviceWillReloadPageOnReturnFromBankIdApp(BankIdSupportedDevice detectedDevice);
    }
}
