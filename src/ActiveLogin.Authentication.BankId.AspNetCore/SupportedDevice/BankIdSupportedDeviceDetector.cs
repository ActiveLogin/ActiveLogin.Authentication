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
            var deviceBrowser = GetDeviceBrowser(normalizedUserAgent);
            var deviceType = GetDeviceType(deviceOs);

            return new BankIdSupportedDevice(deviceType, deviceOs, deviceBrowser);
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
                    && !IsEdge(userAgent);
        }

        private static bool IsChrome(string userAgent)
        {
            return IsUserAgent(userAgent, "crios", "chrome")
                    && !IsEdge(userAgent);
        }

        private static bool IsFirefox(string userAgent)
        {
            return IsUserAgent(userAgent, "firefox", "fxios");
        }

        private static bool IsEdge(string userAgent)
        {
            return IsUserAgent(userAgent, "edg", "edge", "edgios", "edga");
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
