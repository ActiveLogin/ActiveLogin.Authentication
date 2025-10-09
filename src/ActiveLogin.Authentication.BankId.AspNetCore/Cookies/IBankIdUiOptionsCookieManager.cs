using ActiveLogin.Authentication.BankId.AspNetCore.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Cookies;

/// <summary>
/// Manages UiOptions storage in cookies using GUID-based keys to reduce URL length.
/// </summary>
public interface IBankIdUiOptionsCookieManager
{
    /// <summary>
    /// Stores the UiOptions in a protected cookie and returns a GUID identifier.
    /// </summary>
    /// <param name="uiOptions">The UiOptions to store.</param>
    /// <returns>A GUID that can be used to retrieve the options later.</returns>
    string Store(BankIdUiOptions uiOptions);

    /// <summary>
    /// Retrieves the UiOptions using a GUID identifier.
    /// </summary>
    /// <param name="guid">The GUID identifier for the stored options.</param>
    /// <returns>The UiOptions, or null if not found or expired.</returns>
    BankIdUiOptions? Retrieve(string guid);

    /// <summary>
    /// Deletes the stored UiOptions for a given GUID.
    /// </summary>
    /// <param name="guid">The GUID identifier for the stored options.</param>
    void Delete(string guid);

    /// <summary>
    /// Checks if a string is a valid GUID format.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <returns>True if the string is a valid GUID format.</returns>
    bool IsGuid(string value);
}
