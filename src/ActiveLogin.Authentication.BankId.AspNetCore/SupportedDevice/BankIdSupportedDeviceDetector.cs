using System;
using System.Linq;

namespace ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice
{
    public class BankIdSupportedDeviceDetector : IBankIdSupportedDeviceDetector
    {
        public BankIdSupportedDevice Detect(string userAgent)
        {
            var normalizedUserAgent = userAgent?.ToLower().Trim() ?? string.Empty;

            var deviceOs = GetDeviceOs(normalizedUserAgent);
            var deviceOsVersion = GetDeviceOsVersion(normalizedUserAgent);
            var deviceBrowser = GetDeviceBrowser(normalizedUserAgent);
            var deviceType = GetDeviceType(deviceOs);

            return new BankIdSupportedDevice(deviceType, deviceOs, deviceBrowser, deviceOsVersion);
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

        private static BankIdSupportedDeviceOs GetDeviceOs(string userAgent)
        {
            if (IsIos(userAgent))
            {
                return BankIdSupportedDeviceOs.Ios;
            }

            if (IsAndroid(userAgent))
            {
                return BankIdSupportedDeviceOs.Android;
            }

            if (IsWindowsDesktop(userAgent))
            {
                return BankIdSupportedDeviceOs.Windows;
            }

            if (IsMacOs(userAgent))
            {
                return BankIdSupportedDeviceOs.MacOs;
            }

            if (IsWindowsPhone(userAgent))
            {
                return BankIdSupportedDeviceOs.WindowsPhone;
            }

            return BankIdSupportedDeviceOs.Unknown;
        }

        private static BankIdSupportedDeviceOsVersion GetDeviceOsVersion(string userAgent)
        {
            return IsAndroid(userAgent) ? GetAndroidVersion(userAgent) : BankIdSupportedDeviceOsVersion.Empty;
        }

        private static BankIdSupportedDeviceOsVersion GetAndroidVersion(string userAgent)
        {
            if(string.IsNullOrWhiteSpace(userAgent))
            {
                return BankIdSupportedDeviceOsVersion.Empty;
            }

            try
            {
                //Example userAgent for Android "Mozilla/5.0 (Linux; Android 6.0.1; SM-G532G Build/MMB29T) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.83 Mobile Safari/537.36";
                const string Android = "android";

                var versionNumber = userAgent.Substring(userAgent.IndexOf(Android, StringComparison.Ordinal) + Android.Length).Trim();
                versionNumber = versionNumber.Substring(0, versionNumber.IndexOf(";", StringComparison.Ordinal));
                var versionNumberList = versionNumber.Split('.').Select(v => int.TryParse(v, out var version) ? version : 0).ToArray();

                return versionNumberList.Length switch
                {
                    1 => new BankIdSupportedDeviceOsVersion(versionNumberList[0]),
                    2 => new BankIdSupportedDeviceOsVersion(versionNumberList[0], versionNumberList[1]),
                    3 => new BankIdSupportedDeviceOsVersion(versionNumberList[0], versionNumberList[1], versionNumberList[2]),
                    _ => BankIdSupportedDeviceOsVersion.Empty
                };
            }
            catch
            {
                return BankIdSupportedDeviceOsVersion.Empty;
            }
        }

        private static BankIdSupportedDeviceBrowser GetDeviceBrowser(string userAgent)
        {
            if (IsSafari(userAgent))
            {
                return BankIdSupportedDeviceBrowser.Safari;
            }

            if (IsChrome(userAgent))
            {
                return BankIdSupportedDeviceBrowser.Chrome;
            }

            if (IsFirefox(userAgent))
            {
                return BankIdSupportedDeviceBrowser.Firefox;
            }

            if (IsEdge(userAgent))
            {
                return BankIdSupportedDeviceBrowser.Edge;
            }

            if (IsSamsungBrowser(userAgent))
            {
                return BankIdSupportedDeviceBrowser.SamsungBrowser;
            }

            if (IsOpera(userAgent))
            {
                return BankIdSupportedDeviceBrowser.Opera;
            }

            return BankIdSupportedDeviceBrowser.Unknown;
        }

        private static bool IsIos(string userAgent)
        {
            if (IsWindowsPhone(userAgent))
            {
                return false;
            }

            return userAgent.Contains("ipad;")
                   || userAgent.Contains("iphone;");
        }

        private static bool IsAndroid(string userAgent)
        {
            if (IsWindowsPhone(userAgent))
            {
                return false;
            }

            return userAgent.Contains("android");
        }

        private static bool IsWindowsPhone(string userAgent)
        {
            return userAgent.Contains("windows phone");
        }

        private static bool IsWindowsDesktop(string userAgent)
        {
            if (IsWindowsPhone(userAgent))
            {
                return false;
            }

            return userAgent.Contains("windows");
        }

        private static bool IsMacOs(string userAgent)
        {
            if (IsIos(userAgent))
            {
                return false;
            }

            return userAgent.Contains("mac os");
        }

        private static bool IsSafari(string userAgent)
        {
            return IsUserAgent(userAgent, "safari")
                    && !IsChrome(userAgent)
                    && !IsFirefox(userAgent)
                    && !IsEdge(userAgent)
                    && !IsSamsungBrowser(userAgent)
                    && !IsOpera(userAgent);
        }

        private static bool IsChrome(string userAgent)
        {
            return IsUserAgent(userAgent, "crios", "chrome")
                    && !IsEdge(userAgent)
                    && !IsSamsungBrowser(userAgent)
                    && !IsOpera(userAgent);
        }

        private static bool IsFirefox(string userAgent)
        {
            return IsUserAgent(userAgent, "firefox", "fxios");
        }

        private static bool IsEdge(string userAgent)
        {
            return IsUserAgent(userAgent, "edg", "edge", "edgios", "edga");
        }

        private static bool IsSamsungBrowser(string userAgent)
        {
            return IsUserAgent(userAgent, "samsungbrowser");
        }

        private static bool IsOpera(string userAgent)
        {
            return IsUserAgent(userAgent, "opr");
        }

        private static bool IsUserAgent(string userAgent, params string[] keys)
        {
            return keys.Any(x => IsUserAgent(userAgent, x));
        }

        private static bool IsUserAgent(string userAgent, string key)
        {
            var fullKey = key + "/";
            return userAgent.Contains(fullKey, StringComparison.InvariantCulture);
        }
    }
}
