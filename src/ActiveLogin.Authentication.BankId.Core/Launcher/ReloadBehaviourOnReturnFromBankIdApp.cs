namespace ActiveLogin.Authentication.BankId.Core.Launcher;

/// <summary>
/// Represents the reload behaviour of the device when returning from the BankID app.
/// </summary>
public enum ReloadBehaviourOnReturnFromBankIdApp
{
    /// <summary>
    /// The device will reload the page for Safari on iOS.
    /// </summary>
    Default = 0,

    /// <summary>
    /// The device will always reload the page.
    /// </summary>
    Always,

    /// <summary>
    /// The device will never reload the page.
    /// </summary>
    Never
}
