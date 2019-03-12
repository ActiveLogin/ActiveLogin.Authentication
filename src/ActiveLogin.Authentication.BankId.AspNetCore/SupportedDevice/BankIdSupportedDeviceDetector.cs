namespace ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice
{
    public class BankIdSupportedDeviceDetector : IBankIdSupportedDeviceDetector
    {
        public BankIdSupportedDevice Detect(string userAgent)
        {
            string normalizedUserAgent = userAgent?.ToLower().Trim() ?? string.Empty;

            bool isIos = IsIos(normalizedUserAgent);
            bool isAndroid = IsAndroid(normalizedUserAgent);
            bool isWindowsPhone = IsWindowsPhone(normalizedUserAgent);

            bool isWindowsDesktop = IsWindowsDesktop(normalizedUserAgent);
            bool isMacOs = IsMacOs(normalizedUserAgent);

            bool isMobile = isIos || isAndroid || isWindowsPhone;
            bool isDesktop = isWindowsDesktop || isMacOs;

            return new BankIdSupportedDevice(isMobile, isDesktop, isIos, isAndroid, isWindowsPhone, isWindowsDesktop,
                isMacOs);
        }

        private bool IsIos(string userAgent)
        {
            if (IsWindowsPhone(userAgent))
                return false;

            return userAgent.Contains("ipad;")
                   || userAgent.Contains("iphone;");
        }

        private bool IsAndroid(string userAgent)
        {
            if (IsWindowsPhone(userAgent))
                return false;

            return userAgent.Contains("android");
        }

        private bool IsWindowsPhone(string userAgent)
        {
            return userAgent.Contains("windows phone");
        }

        private bool IsWindowsDesktop(string userAgent)
        {
            if (IsWindowsPhone(userAgent))
                return false;

            return userAgent.Contains("windows");
        }

        private bool IsMacOs(string userAgent)
        {
            if (IsIos(userAgent))
                return false;

            return userAgent.Contains("mac os");
        }
    }
}
