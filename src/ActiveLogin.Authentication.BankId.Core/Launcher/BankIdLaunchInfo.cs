namespace ActiveLogin.Authentication.BankId.Core.Launcher;

public class BankIdLaunchInfo
{
    public BankIdLaunchInfo(string launchUrl, bool deviceMightRequireUserInteractionToLaunchBankIdApp, bool deviceWillReloadPageOnReturnFromBankIdApp, string? returnUrl = null)
    {
        LaunchUrl = launchUrl;
        DeviceMightRequireUserInteractionToLaunchBankIdApp = deviceMightRequireUserInteractionToLaunchBankIdApp;
        DeviceWillReloadPageOnReturnFromBankIdApp = deviceWillReloadPageOnReturnFromBankIdApp;
        ReturnUrl = returnUrl;
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

    /// <summary>
    /// Orders started on the same device as where the user's BankID is stored (started with autostart token)
    /// will call this URL when the order is completed.
    /// 
    /// Any return URL provided in the start URL when the BankID app was launched will be ignored.
    /// If the user has a version of the BankID app that does not support getting the returnUrl from the server,
    /// the order will be cancelled and the user will be asked to update their app.
    /// 
    /// The return URL you provide should include a nonce to the session. When the user returns to your app or web page,
    /// your service should verify that the order was completed successfully and that the device receiving the returnUrl
    /// is the same device that started the order.
    /// 
    /// Using this parameter will make your service more secure and strengthen the channel binding between you and the user.
    /// 
    /// Ensure that the cookie or user IP address has not changed from the starting page to the returnUrl page.
    /// </summary>
    public string? ReturnUrl { get; }
}
