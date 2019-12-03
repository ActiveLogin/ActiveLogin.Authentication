using System;
using System.Linq;

namespace ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice
{
    public class BankIdSupportedDeviceDetector : IBankIdSupportedDeviceDetector
    {
        public BankIdSupportedDevice Detect(string userAgent)
        {
            var normalizedUserAgent = userAgent?.ToLower().Trim() ?? string.Empty;

            var isIos = IsIos(normalizedUserAgent);
            var isAndroid = IsAndroid(normalizedUserAgent);
            var isWindowsPhone = IsWindowsPhone(normalizedUserAgent);

            var isWindowsDesktop = IsWindowsDesktop(normalizedUserAgent);
            var isMacOs = IsMacOs(normalizedUserAgent);

            var isMobile = isIos || isAndroid || isWindowsPhone;
            var isDesktop = isWindowsDesktop || isMacOs;

            var isSafari = IsSafari(normalizedUserAgent);
            var isChrome = IsChrome(normalizedUserAgent);
            var isFirefox = IsFirefox(normalizedUserAgent);
            var isEdge = IsEdge(normalizedUserAgent);

            return new BankIdSupportedDevice(isMobile, isDesktop, isIos, isAndroid, isWindowsPhone, isWindowsDesktop, isMacOs, isSafari, isChrome, isFirefox, isEdge);
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
