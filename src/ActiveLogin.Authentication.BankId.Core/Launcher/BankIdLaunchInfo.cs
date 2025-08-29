using ActiveLogin.Authentication.BankId.Api.Models;

namespace ActiveLogin.Authentication.BankId.Core.Launcher;

public class BankIdLaunchInfo
{
    /// <summary>
    /// Creates a new <see cref="BankIdLaunchInfo"/> instance.
    /// </summary>
    /// <param name="launchUrl">The URL used to launch the BankID app.</param>
    /// <param name="deviceMightRequireUserInteractionToLaunchBankIdApp">
    /// True if the device/browser might require user interaction, such as a button click, to launch the BankID app.
    /// </param>
    /// <param name="deviceWillReloadPageOnReturnFromBankIdApp">
    /// True if the device/browser will reload the page when returning from the BankID app.
    /// </param>
    /// <param name="returnUrl">
    /// The return URL to send to BankID as part of the backend call when creating the order (auth, sign, or payment).
    /// This URL will be called when the order completes. It is recommended to provide the return URL
    /// via the backend call rather than including it in the autolaunch URL.
    /// </param>
    public BankIdLaunchInfo(
        string launchUrl,
        bool deviceMightRequireUserInteractionToLaunchBankIdApp,
        bool deviceWillReloadPageOnReturnFromBankIdApp,
        string returnUrl)
    {
        LaunchUrl = launchUrl;
        DeviceMightRequireUserInteractionToLaunchBankIdApp = deviceMightRequireUserInteractionToLaunchBankIdApp;
        DeviceWillReloadPageOnReturnFromBankIdApp = deviceWillReloadPageOnReturnFromBankIdApp;
        ReturnUrl = returnUrl;
    }

    /// <summary>
    /// DEPRECATED: Use the constructor that requires returnUrl.
    /// This overload will be removed in a future release.
    /// </summary>
    /// <param name="launchUrl">The URL used to launch the BankID app.</param>
    /// <param name="deviceMightRequireUserInteractionToLaunchBankIdApp">
    /// True if the device/browser might require user interaction, such as a button click, to launch the BankID app.
    /// </param>
    /// <param name="deviceWillReloadPageOnReturnFromBankIdApp">
    /// True if the device/browser will reload the page when returning from the BankID app.
    /// </param>
    [Obsolete("Use the constructor that requires 'returnUrl'. This overload will be removed in a future release.")]
    public BankIdLaunchInfo(
        string launchUrl,
        bool deviceMightRequireUserInteractionToLaunchBankIdApp,
        bool deviceWillReloadPageOnReturnFromBankIdApp)
    {
        LaunchUrl = launchUrl;
        DeviceMightRequireUserInteractionToLaunchBankIdApp = deviceMightRequireUserInteractionToLaunchBankIdApp;
        DeviceWillReloadPageOnReturnFromBankIdApp = deviceWillReloadPageOnReturnFromBankIdApp;
        ReturnUrl = null;
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
    /// Return URL used when the order is started on the same device
    /// where the user’s BankID is installed (i.e using the <see cref="LaunchUrl"/>).
    /// When the order completes, the BankID app will redirect to this URL.
    ///
    /// If both a return URL is specified here and one was provided as part of the <see cref="LaunchUrl"/>,
    /// using the redirect parameter. BankID will ignore the one in the redirect paramter and use this value instead.
    /// 
    /// If the user has a version of the BankID app that does not support getting the returnUrl from the server,
    /// the order will be cancelled and the user will be asked to update their app.
    /// 
    /// The return URL you provide should include a nonce to the session.
    /// When the user returns to your app or web page, your service should verify that
    /// the device receiving the returnUrl is the same device that started the order.
    /// </summary>
    public string? ReturnUrl { get; }
}
