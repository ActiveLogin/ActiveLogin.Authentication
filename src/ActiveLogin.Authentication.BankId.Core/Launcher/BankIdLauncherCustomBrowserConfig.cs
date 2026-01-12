namespace ActiveLogin.Authentication.BankId.Core.Launcher;

public record BrowserScheme(string scheme)
{
    // we need to trim any trailing :// to make it easier to use
    private readonly string _value = scheme.TrimEnd(':', '/');

    public override string ToString() => _value;
    public static implicit operator string(BrowserScheme browserScheme) => browserScheme._value;
}

public class BankIdLauncherCustomBrowserConfig
{
    public BankIdLauncherCustomBrowserConfig(
        BrowserScheme browserScheme,
        BrowserMightRequireUserInteractionToLaunch browserMightRequireUserInteractionToLaunch
    )
    {
        BrowserScheme = browserScheme;
        BrowserMightRequireUserInteractionToLaunch = browserMightRequireUserInteractionToLaunch;
    }

    [Obsolete("Specifying ReturnUrl is deprecated, use BrowserScheme instead.")]
    public BankIdLauncherCustomBrowserConfig(
        string? returnUrl,
        BrowserReloadBehaviourOnReturnFromBankIdApp browserReloadBehaviourOnReturnFromBankIdApp = BrowserReloadBehaviourOnReturnFromBankIdApp.Default,
        BrowserMightRequireUserInteractionToLaunch browserMightRequireUserInteractionToLaunch = BrowserMightRequireUserInteractionToLaunch.Default
    )
    {
        ReturnUrl = returnUrl;
        BrowserReloadBehaviourOnReturnFromBankIdApp = browserReloadBehaviourOnReturnFromBankIdApp;
        BrowserMightRequireUserInteractionToLaunch = browserMightRequireUserInteractionToLaunch;
    }

    /// <summary>
    /// The URL that the BankID app will try to launch once the BankID app is finished.
    /// Set to null to fall back to default Active Login behaviour.
    /// Set to empty string to not launch any URL, and instead the BanKID app will ask the user to open the last app.
    /// This will only be applied to iOS as Android automatically launches the previous app.
    /// </summary>
    [Obsolete("Deprecated in favor of only specifying BrowserScheme")]
    public string? ReturnUrl { get; set; }
    public BrowserScheme? BrowserScheme { get; set; }

    /// <summary>
    /// The reload behaviour of the browser when returning from the BankID app.
    /// </summary>
    [Obsolete("This setting is deprecated and will be removed in future versions.")]
    public BrowserReloadBehaviourOnReturnFromBankIdApp BrowserReloadBehaviourOnReturnFromBankIdApp { get; set; }

    /// <summary>
    /// If the browser might require user interaction to launch the BankID app.
    /// </summary>
    public BrowserMightRequireUserInteractionToLaunch BrowserMightRequireUserInteractionToLaunch { get; set; }
}
