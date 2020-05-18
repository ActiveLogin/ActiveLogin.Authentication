namespace ActiveLogin.Authentication.BankId.AspNetCore.Launcher
{
    public class BankIdLaunchInfo
    {
        public BankIdLaunchInfo(string launchUrl, bool deviceMightRequireUserInteractionToLaunchBankIdApp, bool deviceWillReloadPageOnReturnFromBankIdApp)
        {
            LaunchUrl = launchUrl;
            DeviceMightRequireUserInteractionToLaunchBankIdApp = deviceMightRequireUserInteractionToLaunchBankIdApp;
            DeviceWillReloadPageOnReturnFromBankIdApp = deviceWillReloadPageOnReturnFromBankIdApp;
        }

        /// <summary>
        /// Returns the url used to launch the BankID app.
        /// </summary>
        public string LaunchUrl { get; }

        /// <summary>
        /// If the device/browser might require user interaction, such as button click, to launch a third party app.
        /// </summary>
        /// <returns></returns>
        public bool DeviceMightRequireUserInteractionToLaunchBankIdApp { get; }

        /// <summary>
        /// If the device/browser will reload the page on return from the BankID app.
        /// </summary>
        /// <returns></returns>
        public bool DeviceWillReloadPageOnReturnFromBankIdApp { get; }
    }
}
