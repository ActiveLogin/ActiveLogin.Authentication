using ActiveLogin.Authentication.BankId.AspNetCore.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Cookies;

/// <summary>
/// Manages UiOptions storage in cookies using GUID-based keys to reduce URL length.
/// </summary>
public interface IBankIdUiOptionsCookieManager
{
    /// <summary>
    /// Stores the UiOptions in a protected cookie.
    /// </summary>
    /// <param name="uiOptions">The UiOptions to store.</param>
    /// <param name="expiresFrom">The base <see cref="DateTimeOffset"/> from which the cookie's expiration is calculated.</param>
    void Store(BankIdUiOptions uiOptions, DateTimeOffset expiresFrom);

    /// <summary>
    /// Retrieves the UiOptions from the cookie and unprotects it.
    /// </summary>
    /// <returns>The UiOptions, or null if not found or expired.</returns>
    BankIdUiOptions? Retrieve();

    /// <summary>
    /// Deletes the stored UiOptions cookie.
    /// </summary>
    void Delete();
}
