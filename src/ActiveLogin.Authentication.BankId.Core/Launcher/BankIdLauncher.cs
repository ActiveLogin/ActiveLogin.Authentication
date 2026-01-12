using System.Text;

using ActiveLogin.Authentication.BankId.Core.Helpers;
using ActiveLogin.Authentication.BankId.Core.SupportedDevice;

namespace ActiveLogin.Authentication.BankId.Core.Launcher;

internal class BankIdLauncher(
    IBankIdSupportedDeviceDetector bankIdSupportedDeviceDetector,
    ICustomBrowserResolver customBrowserResolver
) : IBankIdLauncher
{
    private const string BankIdSchemePrefix = "bankid:///";
    private const string BankIdAppLinkPrefix = "https://app.bankid.com/";

    private const string BankIdAutoStartTokenQueryStringParamName = "autostarttoken";
    private const string BankIdRpRefQueryStringParamName = "rpref";

    private readonly IBankIdSupportedDeviceDetector _bankIdSupportedDeviceDetector = bankIdSupportedDeviceDetector;

    public async Task<BankIdLaunchInfo> GetLaunchInfoAsync(LaunchUrlRequest request)
    {
        var detectedDevice = _bankIdSupportedDeviceDetector.Detect();
        var customBrowserConfig = await customBrowserResolver.GetConfig(request);

        var deviceMightRequireUserInteractionToLaunch = GetDeviceMightRequireUserInteractionToLaunchBankIdApp(detectedDevice, customBrowserConfig);
        var launchUrl = GetLaunchUrl(detectedDevice, request);

        return new BankIdLaunchInfo(launchUrl, deviceMightRequireUserInteractionToLaunch);
    }

    private bool GetDeviceMightRequireUserInteractionToLaunchBankIdApp(BankIdSupportedDevice detectedDevice, BankIdLauncherCustomBrowserConfig? customBrowserConfig)
    {
        var userInteractionBehaviour = customBrowserConfig?.BrowserMightRequireUserInteractionToLaunch ?? BrowserMightRequireUserInteractionToLaunch.Default;

        return userInteractionBehaviour switch
        {
            BrowserMightRequireUserInteractionToLaunch.Always => true,
            BrowserMightRequireUserInteractionToLaunch.Never => false,

            // On Android, some browsers will (for security reasons) not launching a
            // third party app/scheme (BankID) if there is no user interaction.
            //
            // - Chrome, Edge, Samsung Internet Browser and Brave is confirmed to require User Interaction
            // - Firefox and Opera is confirmed to work without User Interaction
            //
            // On iOS, browsers do not have this restriction
            _ => detectedDevice is {
                DeviceOs: BankIdSupportedDeviceOs.Android,
                DeviceBrowser: not (BankIdSupportedDeviceBrowser.Firefox or BankIdSupportedDeviceBrowser.Opera)
            }
        };
    }

    private static string GetLaunchUrl(BankIdSupportedDevice device, LaunchUrlRequest request)
    {
        var prefix = GetPrefixPart(device);
        var queryString = GetQueryStringPart(request);

        return $"{prefix}{queryString}";
    }

    private static string GetPrefixPart(BankIdSupportedDevice device)
    {
        return CanUseAppLink(device)
            ? BankIdAppLinkPrefix
            : BankIdSchemePrefix;
    }

    private static bool CanUseAppLink(BankIdSupportedDevice device)
    {
        // Only Safari on IOS and Chrome or Edge on Android version >= 6 seems to support
        //  the https://app.bankid.com/ launch url

        return device is
        {
            DeviceOs: BankIdSupportedDeviceOs.Ios,
            DeviceBrowser: BankIdSupportedDeviceBrowser.Safari
        }
        or
        {
            DeviceOs: BankIdSupportedDeviceOs.Android,
            DeviceOsVersion.MajorVersion: >= 6,
            DeviceBrowser: BankIdSupportedDeviceBrowser.Chrome or BankIdSupportedDeviceBrowser.Edge
        };
    }

    private static string GetQueryStringPart(LaunchUrlRequest request)
    {
        var queryStringParams = new Dictionary<string, string>();

        if (!string.IsNullOrWhiteSpace(request.AutoStartToken))
        {
            queryStringParams.Add(BankIdAutoStartTokenQueryStringParamName, request.AutoStartToken);
        }

        if (!string.IsNullOrWhiteSpace(request.RelyingPartyReference))
        {
            queryStringParams.Add(BankIdRpRefQueryStringParamName, Base64Encode(request.RelyingPartyReference));
        }

        return QueryStringGenerator.ToQueryString(queryStringParams);
    }

    private static string Base64Encode(string value)
    {
        var encodedBytes = Encoding.Unicode.GetBytes(value);
        return Convert.ToBase64String(encodedBytes);
    }
}
