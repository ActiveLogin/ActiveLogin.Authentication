namespace ActiveLogin.Authentication.BankId.Core.SupportedDevice;

/// <summary>
/// Detect if the device requesting is one of the supported platforms for BankID.
/// </summary>
public interface IBankIdSupportedDeviceDetector
{
    /// <summary>
    /// Detect the current device.
    /// </summary>
    /// <returns></returns>
    BankIdSupportedDevice Detect();
}
