namespace ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice
{
    public class BankIdSupportedDevice
    {
        public BankIdSupportedDevice(bool isMobile, bool isDesktop, bool isIos, bool isAndroid, bool isWindowsPhone, bool isWindowsDesktop, bool isMacOs, bool isSafari, bool isChrome, bool isFirefox, bool isEdge)
        {
            IsMobile = isMobile;
            IsDesktop = isDesktop;

            IsIos = isIos;
            IsAndroid = isAndroid;
            IsWindowsPhone = isWindowsPhone;

            IsWindowsDesktop = isWindowsDesktop;
            IsMacOs = isMacOs;

            IsSafari = isSafari;
            IsChrome = isChrome;
            IsFirefox = isFirefox;
            IsEdge = isEdge;
        }

        public bool IsMobile { get; }
        public bool IsDesktop { get; }

        public bool IsIos { get; }
        public bool IsAndroid { get; }
        public bool IsWindowsPhone { get; }

        public bool IsWindowsDesktop { get; }
        public bool IsMacOs { get; }

        public bool IsSafari { get; }
        public bool IsChrome { get; }
        public bool IsFirefox { get; }
        public bool IsEdge { get; }
    }
}
