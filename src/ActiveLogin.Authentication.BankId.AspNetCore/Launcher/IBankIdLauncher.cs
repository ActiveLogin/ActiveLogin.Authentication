using ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Launcher
{
    public interface IBankIdLauncher
    {
        /// <summary>
        /// Returns the url used to launch the BankID app.
        /// </summary>
        /// <param name="device">The device that the user is using.</param>
        /// <param name="request">Launch info</param>
        /// <returns></returns>
        string GetLaunchUrl(BankIdSupportedDevice device, LaunchUrlRequest request);

        /// <summary>
        /// If the device/browser might require user interaction, such as button click, to launch a third party app.
        /// </summary>
        /// <param name="detectedDevice"></param>
        /// <returns></returns>
        bool GetDeviceMightRequireUserInteractionToLaunchBankIdApp(BankIdSupportedDevice detectedDevice);

        /// <summary>
        /// If the device/browser will reload the page on return from the BankID app.
        /// </summary>
        /// <param name="detectedDevice"></param>
        /// <returns></returns>
        bool GetDeviceWillReloadPageOnReturnFromBankIdApp(BankIdSupportedDevice detectedDevice);
    }
}
