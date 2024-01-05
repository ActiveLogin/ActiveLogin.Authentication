namespace ActiveLogin.Authentication.BankId.Core.Launcher;

public class BankIdLauncherCustomBrowserConfig
{
    public BankIdLauncherCustomBrowserConfig(string? iosReturnUrl, BrowserReloadBehaviourOnReturnFromBankIdApp browserReloadBehaviourOnReturnFromBankIdApp = BrowserReloadBehaviourOnReturnFromBankIdApp.Default, BrowserMightRequireUserInteractionToLaunch browserMightRequireUserInteractionToLaunch = BrowserMightRequireUserInteractionToLaunch.Default)
    {
        IosReturnUrl = iosReturnUrl;
        BrowserReloadBehaviourOnReturnFromBankIdApp = browserReloadBehaviourOnReturnFromBankIdApp;
        BrowserMightRequireUserInteractionToLaunch = browserMightRequireUserInteractionToLaunch;
    }

    /// <summary>
    /// The URL that the BankID app will try to launch once the BankID app is finished.
    /// Set to null to fall back to default Active Login behaviour.
    /// Set to empty string to not launch any URL, and instead the BanKID app will ask the user to open the last app.
    /// This will only be applied to iOS as Android automatically launches the previous app.
    /// </summary>
    public string? IosReturnUrl { get; set; }

    /// <summary>
    /// The reload behaviour of the browser when returning from the BankID app.
    /// </summary>
    public BrowserReloadBehaviourOnReturnFromBankIdApp BrowserReloadBehaviourOnReturnFromBankIdApp { get; set; }

    /// <summary>
    /// If the browser might require user interaction to launch the BankID app.
    /// </summary>
    public BrowserMightRequireUserInteractionToLaunch BrowserMightRequireUserInteractionToLaunch { get; set; }
}
