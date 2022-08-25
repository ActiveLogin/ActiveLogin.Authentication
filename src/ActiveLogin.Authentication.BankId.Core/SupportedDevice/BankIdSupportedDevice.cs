namespace ActiveLogin.Authentication.BankId.Core.SupportedDevice;

/// <summary>
/// Information about the device that is supported by Active Login.
/// </summary>
public class BankIdSupportedDevice
{
    public BankIdSupportedDevice(BankIdSupportedDeviceType deviceType, BankIdSupportedDeviceOs deviceOs, BankIdSupportedDeviceBrowser deviceBrowser, BankIdSupportedDeviceOsVersion deviceOsVersion)
    {
        DeviceType = deviceType;
        DeviceOs = deviceOs;
        DeviceBrowser = deviceBrowser;
        DeviceOsVersion = deviceOsVersion;
    }

    /// <summary>
    /// Device type (desktop, mobile)
    /// </summary>
    public BankIdSupportedDeviceType DeviceType { get; }

    /// <summary>
    /// Device OS (Windows, iOS, Android etc)
    /// </summary>
    public BankIdSupportedDeviceOs DeviceOs { get; }

    /// <summary>
    /// Device OS version (9.0.0, 1.0 etc).
    /// </summary>
    public BankIdSupportedDeviceOsVersion DeviceOsVersion { get; }

    /// <summary>
    /// Device browser (Chrome, Firefox, Safari, Edge etc)
    /// </summary>
    public BankIdSupportedDeviceBrowser DeviceBrowser { get; }


    public static BankIdSupportedDevice Unknown = new(BankIdSupportedDeviceType.Unknown, BankIdSupportedDeviceOs.Unknown, BankIdSupportedDeviceBrowser.Unknown, BankIdSupportedDeviceOsVersion.Empty);
}
