namespace ActiveLogin.Authentication.BankId.Core.Launcher;

/// <summary>
/// Information about the launch of the BankID app.
/// </summary>
/// <param name="launchUrl">The URL used to launch the BankID app.</param>
/// <param name="deviceMightRequireUserInteractionToLaunchBankIdApp">
/// True if the device/browser might require user interaction, such as a button click, to launch the BankID app.
/// </param>
public class BankIdLaunchInfo(
    string launchUrl,
    bool deviceMightRequireUserInteractionToLaunchBankIdApp)
{

    public BankIdLaunchInfo(string launchUrl, bool deviceMightRequireUserInteractionToLaunchBankIdApp, bool deviceWillReloadPageOnReturnFromBankIdApp)
        : this(launchUrl, deviceMightRequireUserInteractionToLaunchBankIdApp)
    {
        // For backward compatibility
    }

    /// <summary>
    /// Returns the url used to launch the BankID app.
    /// </summary>
    public string LaunchUrl { get; } = launchUrl;

    /// <summary>
    /// If the device/browser might require user interaction, such as button click, to launch a third party app.
    /// </summary>
    /// <returns></returns>
    public bool DeviceMightRequireUserInteractionToLaunchBankIdApp { get; } = deviceMightRequireUserInteractionToLaunchBankIdApp;

    /// <summary>
    /// If the device/browser will reload the page on return from the BankID app.
    /// </summary>
    /// <returns></returns>
    [Obsolete("This will be determined by the returnUrl sent to BankId")]
    public bool DeviceWillReloadPageOnReturnFromBankIdApp { get; }
}
