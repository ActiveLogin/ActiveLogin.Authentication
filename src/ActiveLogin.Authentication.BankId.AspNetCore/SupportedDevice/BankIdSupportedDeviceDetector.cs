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

            return new BankIdSupportedDevice(isMobile, isDesktop, isIos, isAndroid, isWindowsPhone, isWindowsDesktop, isMacOs);
        }

        private bool IsIos(string userAgent)
        {
            if (IsWindowsPhone(userAgent))
            {
                return false;
            }

            return userAgent.Contains("ipad;")
                   || userAgent.Contains("iphone;");
        }

        private bool IsAndroid(string userAgent)
        {
            if (IsWindowsPhone(userAgent))
            {
                return false;
            }

            return userAgent.Contains("android");
        }

        private bool IsWindowsPhone(string userAgent)
        {
            return userAgent.Contains("windows phone");
        }

        private bool IsWindowsDesktop(string userAgent)
        {
            if (IsWindowsPhone(userAgent))
            {
                return false;
            }

            return userAgent.Contains("windows");
        }

        private bool IsMacOs(string userAgent)
        {
            if (IsIos(userAgent))
            {
                return false;
            }

            return userAgent.Contains("mac os");
        }
    }
}
