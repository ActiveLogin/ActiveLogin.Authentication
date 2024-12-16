namespace ActiveLogin.Authentication.BankId.Core.UserContext.Device;
public enum BankIdEndUserDeviceType
{
    /// <summary>
    /// The software that launches BankId authentication is a web browser.
    /// </summary>
    Web = 1,

    /// <summary>
    /// The software that launches BankId authentication is a mobile app.
    /// </summary>
    App = 2
}
