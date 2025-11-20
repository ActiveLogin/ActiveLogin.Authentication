using ActiveLogin.Authentication.BankId.Core.SupportedDevice;

namespace ActiveLogin.Authentication.BankId.Core.Test;

/// <summary>
/// Shared test fixture providing predefined BankIdSupportedDevice instances
/// for use across unit tests. Covers the major browsers and platforms documented
/// in the BankID integration guide.
/// </summary>
public static class BankIdTestDevices
{
    /// <summary>
    /// Use this when the specific device doesn't matter for the test scenario.
    /// Makes it clear that device selection is arbitrary and not part of what's being tested.
    /// </summary>
    public static BankIdSupportedDevice AnyDevice => Desktop.Windows_Edge;

    public static class Desktop
    {
        public static BankIdSupportedDevice Windows_Edge =>
            new(BankIdSupportedDeviceType.Desktop, BankIdSupportedDeviceOs.Windows, BankIdSupportedDeviceBrowser.Edge, new BankIdSupportedDeviceOsVersion(10));

        public static BankIdSupportedDevice Windows_Chrome =>
            new(BankIdSupportedDeviceType.Desktop, BankIdSupportedDeviceOs.Windows, BankIdSupportedDeviceBrowser.Chrome, new BankIdSupportedDeviceOsVersion(10));

        public static BankIdSupportedDevice Windows_Firefox =>
            new(BankIdSupportedDeviceType.Desktop, BankIdSupportedDeviceOs.Windows, BankIdSupportedDeviceBrowser.Firefox, new BankIdSupportedDeviceOsVersion(10));

        public static BankIdSupportedDevice MacOs_Safari =>
            new(BankIdSupportedDeviceType.Desktop, BankIdSupportedDeviceOs.MacOs, BankIdSupportedDeviceBrowser.Safari, new BankIdSupportedDeviceOsVersion(10, 15));

        public static BankIdSupportedDevice MacOs_Chrome =>
            new(BankIdSupportedDeviceType.Desktop, BankIdSupportedDeviceOs.MacOs, BankIdSupportedDeviceBrowser.Chrome, new BankIdSupportedDeviceOsVersion(10, 15));
    }

    public static class Mobile
    {
        public static class Android
        {
            public static BankIdSupportedDevice Chrome =>
                new(BankIdSupportedDeviceType.Mobile, BankIdSupportedDeviceOs.Android, BankIdSupportedDeviceBrowser.Chrome, new BankIdSupportedDeviceOsVersion(10));

            public static BankIdSupportedDevice Edge =>
                new(BankIdSupportedDeviceType.Mobile, BankIdSupportedDeviceOs.Android, BankIdSupportedDeviceBrowser.Edge, new BankIdSupportedDeviceOsVersion(10));

            public static BankIdSupportedDevice SamsungInternet =>
                new(BankIdSupportedDeviceType.Mobile, BankIdSupportedDeviceOs.Android, BankIdSupportedDeviceBrowser.SamsungBrowser, new BankIdSupportedDeviceOsVersion(10));

            public static BankIdSupportedDevice Firefox =>
                new(BankIdSupportedDeviceType.Mobile, BankIdSupportedDeviceOs.Android, BankIdSupportedDeviceBrowser.Firefox, new BankIdSupportedDeviceOsVersion(10));

            public static BankIdSupportedDevice Opera =>
                new(BankIdSupportedDeviceType.Mobile, BankIdSupportedDeviceOs.Android, BankIdSupportedDeviceBrowser.Opera, new BankIdSupportedDeviceOsVersion(10));
        }

        public static class Ios
        {
            public static BankIdSupportedDevice Safari =>
                new(BankIdSupportedDeviceType.Mobile, BankIdSupportedDeviceOs.Ios, BankIdSupportedDeviceBrowser.Safari, new BankIdSupportedDeviceOsVersion(14));

            public static BankIdSupportedDevice Chrome =>
                new(BankIdSupportedDeviceType.Mobile, BankIdSupportedDeviceOs.Ios, BankIdSupportedDeviceBrowser.Chrome, new BankIdSupportedDeviceOsVersion(14));

            public static BankIdSupportedDevice Firefox =>
                new(BankIdSupportedDeviceType.Mobile, BankIdSupportedDeviceOs.Ios, BankIdSupportedDeviceBrowser.Firefox, new BankIdSupportedDeviceOsVersion(14));

            public static BankIdSupportedDevice Edge =>
                new(BankIdSupportedDeviceType.Mobile, BankIdSupportedDeviceOs.Ios, BankIdSupportedDeviceBrowser.Edge, new BankIdSupportedDeviceOsVersion(14));

            public static BankIdSupportedDevice Opera =>
                new(BankIdSupportedDeviceType.Mobile, BankIdSupportedDeviceOs.Ios, BankIdSupportedDeviceBrowser.Opera, new BankIdSupportedDeviceOsVersion(14));
        }
    }
}
