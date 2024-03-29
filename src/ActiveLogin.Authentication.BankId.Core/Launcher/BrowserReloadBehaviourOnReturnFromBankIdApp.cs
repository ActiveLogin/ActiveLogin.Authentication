namespace ActiveLogin.Authentication.BankId.Core.Launcher;

/// <summary>
/// Represents the reload behaviour of the browser when returning from the BankID app.
/// </summary>
public enum BrowserReloadBehaviourOnReturnFromBankIdApp
{
    /// <summary>
    /// Use the default implementation in Active Login.
    /// </summary>
    Default = 0,

    /// <summary>
    /// The browser will always reload the page.
    /// </summary>
    Always,

    /// <summary>
    /// The browser will never reload the page.
    /// </summary>
    Never
}
