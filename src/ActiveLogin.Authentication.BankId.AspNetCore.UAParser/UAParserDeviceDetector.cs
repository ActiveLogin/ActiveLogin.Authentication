using ActiveLogin.Authentication.BankId.Core.SupportedDevice;

using Microsoft.AspNetCore.Http;

using UAParser;

namespace ActiveLogin.Authentication.BankId.AspNetCore.UAParser;

/// <summary>
/// This class is used for device and browser detection to support requirements in the
/// BankID Relaying party Guidelines (e.g. use of different launch URLs for the BankID app
/// for different devices and browsers).
/// </summary>
/// <remarks>
/// It uses the ua_parser C# library for user agent parsing.
/// </remarks>
public class UAParserDeviceDetector : IBankIdSupportedDeviceDetectorByUserAgent
{
    public BankIdSupportedDevice Detect(string userAgent)
    {
        var uaParser = Parser.GetDefault();
        var clientInfo = uaParser.Parse(userAgent);

        var deviceType = GetDeviceType(clientInfo);
        var deviceOs = GetDeviceOs(clientInfo);
        var deviceOsVersion = GetDeviceOsVersion(clientInfo);
        var deviceBrowser = GetDeviceBrowser(clientInfo);

        return new BankIdSupportedDevice(deviceType, deviceOs, deviceBrowser, deviceOsVersion);
    }

    private static BankIdSupportedDeviceBrowser GetDeviceBrowser(ClientInfo clientInfo) => clientInfo switch
    {
        _ when IsEdge(clientInfo) => BankIdSupportedDeviceBrowser.Edge,
        _ when IsSamsungBrowser(clientInfo) => BankIdSupportedDeviceBrowser.SamsungBrowser,
        _ when IsOpera(clientInfo) => BankIdSupportedDeviceBrowser.Opera,
        _ when IsFirefox(clientInfo) => BankIdSupportedDeviceBrowser.Firefox,

        _ when IsSafari(clientInfo) => BankIdSupportedDeviceBrowser.Safari,
        _ when IsChrome(clientInfo) => BankIdSupportedDeviceBrowser.Chrome,

        _ => BankIdSupportedDeviceBrowser.Unknown
    };

    private static BankIdSupportedDeviceOs GetDeviceOs(ClientInfo clientInfo) => clientInfo.OS switch
    {
        var os when IsIos(os) => BankIdSupportedDeviceOs.Ios,
        var os when IsAndroid(os) => BankIdSupportedDeviceOs.Android,
        var os when IsWindowsPhone(os) => BankIdSupportedDeviceOs.WindowsPhone,
        var os when IsWindows(os) => BankIdSupportedDeviceOs.Windows,
        var os when IsMacOs(os) => BankIdSupportedDeviceOs.MacOs,
        _ => BankIdSupportedDeviceOs.Unknown
    };

    private static BankIdSupportedDeviceOsVersion GetDeviceOsVersion(ClientInfo clientInfo)
    {
        var hasMajor = int.TryParse(clientInfo.OS.Major, out var major);
        var hasMinor = int.TryParse(clientInfo.OS.Minor, out var minor);
        var hasPatch = int.TryParse(clientInfo.OS.Patch, out var patch);

        return (hasMajor, hasMinor, hasPatch) switch
        {
            (true, true, true) => new BankIdSupportedDeviceOsVersion(major, minor, patch),
            (true, true, false) => new BankIdSupportedDeviceOsVersion(major, minor),
            (true, false, false) => new BankIdSupportedDeviceOsVersion(major),
            _ => BankIdSupportedDeviceOsVersion.Empty
        };
    }

    private static BankIdSupportedDeviceType GetDeviceType(ClientInfo clientInfo)
    {
        var deviceOs = GetDeviceOs(clientInfo);
        var deviceType = GetDeviceType(deviceOs);
        var isMobileBrowser = IsMobileBrowser(clientInfo.UA);

        return (deviceType, isMobileBrowser) switch
        {
            (BankIdSupportedDeviceType.Unknown, true) => BankIdSupportedDeviceType.Mobile,
            _ => deviceType
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

    private static bool IsChrome(ClientInfo clientInfo)
    {
        return IsBrowser(clientInfo.UA, "chrome");
    }

    private static bool IsSafari(ClientInfo clientInfo)
    {
        return IsBrowser(clientInfo.UA, "safari");
    }

    private static bool IsEdge(ClientInfo clientInfo)
    {
        return IsBrowser(clientInfo.UA, "edge")
               || IsBrowserRaw(clientInfo.String, "edgios");
    }

    private static bool IsFirefox(ClientInfo clientInfo)
    {
        return IsBrowser(clientInfo.UA, "firefox");
    }

    private static bool IsSamsungBrowser(ClientInfo clientInfo)
    {
        return IsBrowser(clientInfo.UA, "samsung");
    }

    private static bool IsOpera(ClientInfo clientInfo)
    {
        return IsBrowser(clientInfo.UA, "opera")
               || IsBrowserRaw(clientInfo.String, "opt");
    }

    private static bool IsMobileBrowser(UserAgent userAgent)
    {
        return IsBrowser(userAgent, "mobile");
    }

    private static bool IsBrowser(UserAgent userAgent, string browser)
    {
        return userAgent.Family.Contains(browser, StringComparison.InvariantCultureIgnoreCase);
    }

    private static bool IsBrowserRaw(string userAgent, string browser)
    {
        var fullKey = browser + "/";
        return userAgent.Contains(fullKey, StringComparison.InvariantCultureIgnoreCase);
    }

    private static bool IsIos(OS os)
    {
        return IsOs(os, "ios");
    }

    private static bool IsAndroid(OS os)
    {
        return IsOs(os, "android");
    }

    private static bool IsWindowsPhone(OS os)
    {
        return IsOs(os, "windows phone");
    }

    private static bool IsWindows(OS os)
    {
        return IsOs(os, "windows");
    }

    private static bool IsMacOs(OS os)
    {
        return IsOs(os, "mac os");
    }

    private static bool IsOs(OS os, string osName)
    {
        return os.Family.Contains(osName, StringComparison.InvariantCultureIgnoreCase);
    }
}
