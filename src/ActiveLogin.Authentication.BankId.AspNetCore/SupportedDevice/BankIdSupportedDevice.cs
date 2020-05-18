namespace ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice
{
    public class BankIdSupportedDevice
    {
        public BankIdSupportedDevice(BankIdSupportedDeviceType deviceType, BankIdSupportedDeviceOs deviceOs, BankIdSupportedDeviceBrowser deviceBrowser, BankIdSupportedDeviceOsVersion deviceOsVersion)
        {
            DeviceType = deviceType;
            DeviceOs = deviceOs;
            DeviceBrowser = deviceBrowser;
            DeviceOsVersion = deviceOsVersion;
        }

        public BankIdSupportedDeviceType DeviceType { get; }
        public BankIdSupportedDeviceOs DeviceOs { get; }
        public BankIdSupportedDeviceOsVersion DeviceOsVersion { get; }
        public BankIdSupportedDeviceBrowser DeviceBrowser { get; }
    }
}
