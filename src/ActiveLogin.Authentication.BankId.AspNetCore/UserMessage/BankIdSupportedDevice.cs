namespace ActiveLogin.Authentication.BankId.AspNetCore.UserMessage
{
    public class BankIdSupportedDevice
    {
        public BankIdSupportedDevice(bool isMobile, bool isDesktop, bool isIos, bool isAndroid, bool isWindowsPhone, bool isWindowsDekstop, bool isMacOs)
        {
            IsMobile = isMobile;
            IsDesktop = isDesktop;
            IsIos = isIos;
            IsAndroid = isAndroid;
            IsWindowsPhone = isWindowsPhone;
            IsWindowsDekstop = isWindowsDekstop;
            IsMacOs = isMacOs;
        }

        public bool IsMobile { get; }
        public bool IsDesktop { get; }

        public bool IsIos { get; }
        public bool IsAndroid { get; }
        public bool IsWindowsPhone { get; }

        public bool IsWindowsDekstop { get; }
        public bool IsMacOs { get; }
    }
}