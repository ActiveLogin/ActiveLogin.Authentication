using System;
using ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice;
using UAParser;

namespace ActiveLogin.Authentication.BankId.AspNetCore.UAParser
{
    public class UAParserDeviceDetector : IBankIdSupportedDeviceDetector
    {
        public BankIdSupportedDevice Detect(string userAgent)
        {
            var uaParser = Parser.GetDefault();
            var clientInfo = uaParser.Parse(userAgent);

            var deviceOs = GetDeviceOs(clientInfo);
            var deviceBrowser = GetDeviceBrowser(clientInfo);
            var deviceOsVersion = GetDeviceOsVersion(clientInfo);
            var deviceType = GetDeviceType(deviceOs); //TODO: Is it possible to dettermine this by checking if clientInfo.UA.Family contains "Mobile"?

            return new BankIdSupportedDevice(deviceType, deviceOs, deviceBrowser, deviceOsVersion);
        }

        private BankIdSupportedDeviceOsVersion GetDeviceOsVersion(ClientInfo clientInfo)
        {
            var hasMajor = int.TryParse(clientInfo.OS.Major, out var major);
            var hasMinor = int.TryParse(clientInfo.OS.Minor, out var minor);
            var hasPatch = int.TryParse(clientInfo.OS.Patch, out var patch);

            if(hasMajor && hasMinor && hasPatch)
            {
                return new BankIdSupportedDeviceOsVersion(major, minor, patch);
            }

            if (hasMajor && hasMinor)
            {
                return new BankIdSupportedDeviceOsVersion(major, minor);
            }

            if (hasMajor)
            {
                return new BankIdSupportedDeviceOsVersion(major);
            }

            return BankIdSupportedDeviceOsVersion.Empty;
        }

        private BankIdSupportedDeviceBrowser GetDeviceBrowser(ClientInfo clientInfo)
        {
            if (IsChrome(clientInfo.UA.Family))
            {
                return BankIdSupportedDeviceBrowser.Chrome;
            }

            if (IsSafari(clientInfo.UA.Family))
            {
                return BankIdSupportedDeviceBrowser.Safari;
            }

            if (IsEdge(clientInfo.UA.Family))
            {
                return BankIdSupportedDeviceBrowser.Edge;
            }

            if (IsFirefox(clientInfo.UA.Family))
            {
                return BankIdSupportedDeviceBrowser.Firefox;
            }


            if (IsSamsungBrowser(clientInfo.UA.Family))
            {
                return BankIdSupportedDeviceBrowser.SamsungBrowser;
            }

            if (IsOpera(clientInfo.UA.Family))
            {
                return BankIdSupportedDeviceBrowser.Opera;
            }

            return BankIdSupportedDeviceBrowser.Unknown;
        }

        private bool IsChrome(string family)
        {
            return family.ToLower().Contains("chrome");
        }

        private bool IsSafari(string family)
        {
            return family.ToLower().Contains("safari");
        }

        private bool IsEdge(string family)
        {
            return family.ToLower().Contains("edge");
        }

        private bool IsFirefox(string family)
        {
            return family.ToLower().Contains("firefox");
        }

        private bool IsSamsungBrowser(string family)
        {
            return family.ToLower().Contains("samsung");
        }

        private bool IsOpera(string family)
        {
            return family.ToLower().Contains("opera");
        }

        //TODO: Use Contains instead.
        private BankIdSupportedDeviceOs GetDeviceOs(ClientInfo clientInfo)
        {
            return clientInfo.OS.Family.ToLower() switch
            {
                "ios" => BankIdSupportedDeviceOs.Ios,
                "android" => BankIdSupportedDeviceOs.Android,
                "windows phone" => BankIdSupportedDeviceOs.WindowsPhone,
                "windows" => BankIdSupportedDeviceOs.Windows,
                "mac os x" => BankIdSupportedDeviceOs.MacOs,
                _ => BankIdSupportedDeviceOs.Unknown
            };
        }

        private BankIdSupportedDeviceType GetDeviceType(ClientInfo clientInfo)
        {
            return clientInfo.Device.Family switch
            {
                "iPhone" => BankIdSupportedDeviceType.Mobile,
                "iPad" => BankIdSupportedDeviceType.Mobile,
                _ => BankIdSupportedDeviceType.Unknown
            };
        }

        private static BankIdSupportedDeviceType GetDeviceType(BankIdSupportedDeviceOs deviceOs)
        {
            return deviceOs switch
            {
                BankIdSupportedDeviceOs.Windows => BankIdSupportedDeviceType.Desktop,
                BankIdSupportedDeviceOs.MacOs => BankIdSupportedDeviceType.Desktop,

                BankIdSupportedDeviceOs.Ios => BankIdSupportedDeviceType.Mobile,
                BankIdSupportedDeviceOs.Android => BankIdSupportedDeviceType.Mobile,
                BankIdSupportedDeviceOs.WindowsPhone => BankIdSupportedDeviceType.Mobile,

                _ => BankIdSupportedDeviceType.Unknown
            };
        }
    }
}
