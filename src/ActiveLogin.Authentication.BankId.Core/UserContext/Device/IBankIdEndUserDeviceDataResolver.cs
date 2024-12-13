using ActiveLogin.Authentication.BankId.Api;

namespace ActiveLogin.Authentication.BankId.Core.UserContext.Device;

/// <summary>
/// Resolves device parameters for the end user.
/// </summary>
public interface IBankIdEndUserDeviceDataResolver
{
    /// <summary>
    /// Get the device type.
    /// </summary>
    BankIdEndUserDeviceType DeviceType { get; }

    /// <summary>
    /// Get the device parameters.
    /// </summary>
    /// <returns></returns>
    Task<IBankIdEndUserDeviceData> GetDeviceDataAsync();

    /// <summary>
    /// Get the device parameters.
    /// </summary>
    /// <returns></returns>
    IBankIdEndUserDeviceData GetDeviceData();
}
