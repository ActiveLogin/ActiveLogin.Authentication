namespace ActiveLogin.Authentication.BankId.Core.Launcher;

/// <summary>
/// Represents the result of a custom app launch.
/// </summary>
public enum BrowserMightRequireUserInteractionToLaunch
{
    /// <summary>
    /// Use the default implementation in Active Login.
    /// </summary>
    Default = 0,

    /// <summary>
    /// The browser will always require user interaction to launch BankID app.
    /// </summary>
    Always,

    /// <summary>
    /// The browser will never require user interaction to launch BankID app.
    /// </summary>
    Never
}
