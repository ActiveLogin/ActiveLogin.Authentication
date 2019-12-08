namespace ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice
{
    public class BankIdSupportedDevice
    {
        public BankIdSupportedDevice(BankIdSupportedDeviceType deviceType, BankIdSupportedDeviceOs deviceOs, BankIdSupportedDeviceBrowser deviceBrowser)
        {
            DeviceType = deviceType;
            DeviceOs = deviceOs;
            DeviceBrowser = deviceBrowser;
        }

        public BankIdSupportedDeviceType DeviceType { get; }
        public BankIdSupportedDeviceOs DeviceOs { get; }
        public BankIdSupportedDeviceBrowser DeviceBrowser { get; }
    }

    public enum BankIdSupportedDeviceType
    {
        Unknown,

        Desktop,
        Mobile
    }

    public enum BankIdSupportedDeviceOs
    {
        Unknown,

        Windows,
        MacOs,

        Ios,
        Android,
        WindowsPhone
    }

    public enum BankIdSupportedDeviceBrowser
    {
        Unknown,

        Chrome,
        Safari,
        Firefox,
        Edge
    }
}
