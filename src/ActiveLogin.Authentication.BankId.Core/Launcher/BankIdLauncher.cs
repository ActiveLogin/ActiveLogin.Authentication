using System.Collections.Generic;
using System.Text;

using ActiveLogin.Authentication.BankId.Core.Helpers;
using ActiveLogin.Authentication.BankId.Core.SupportedDevice;

namespace ActiveLogin.Authentication.BankId.Core.Launcher;

internal class BankIdLauncher : IBankIdLauncher
{
    private const string BankIdSchemePrefix = "bankid:///";
    private const string BankIdAppLinkPrefix = "https://app.bankid.com/";

    private const string BankIdAutoStartTokenQueryStringParamName = "autostarttoken";
    private const string BankIdRpRefQueryStringParamName = "rpref";
    private const string BankIdRedirectQueryStringParamName = "redirect";

    private const string NullRedirectUrl = "null";

    private const string IosChromeScheme = "googlechromes://";
    private const string IosFirefoxScheme = "firefox://";

    private readonly IBankIdSupportedDeviceDetector _bankIdSupportedDeviceDetector;
    private readonly List<IBankIdLauncherCustomAppCallback> _customAppCallbacks;
    private readonly IReloadPageOnReturnFromBankIdApp? _reloadPageOnReturnFromBankIdApp;

    public BankIdLauncher(IBankIdSupportedDeviceDetector bankIdSupportedDeviceDetector, IEnumerable<IBankIdLauncherCustomAppCallback> customAppCallbacks, IReloadPageOnReturnFromBankIdApp? reloadPageOnReturnFromBankIdApp = null)
    {
        _bankIdSupportedDeviceDetector = bankIdSupportedDeviceDetector;
        _customAppCallbacks = customAppCallbacks.ToList();
        _reloadPageOnReturnFromBankIdApp = reloadPageOnReturnFromBankIdApp;
    }

    public async Task<BankIdLaunchInfo> GetLaunchInfoAsync(LaunchUrlRequest request)
    {
        var detectedDevice = _bankIdSupportedDeviceDetector.Detect();
        var deviceMightRequireUserInteractionToLaunch = GetDeviceMightRequireUserInteractionToLaunchBankIdApp(detectedDevice);
        var deviceWillReloadPageOnReturn = GetDeviceWillReloadPageOnReturnFromBankIdApp(detectedDevice);

        var launchUrl = await GetLaunchUrl(detectedDevice, request, _customAppCallbacks);
        return new BankIdLaunchInfo(launchUrl, deviceMightRequireUserInteractionToLaunch, deviceWillReloadPageOnReturn);
    }

    private bool GetDeviceMightRequireUserInteractionToLaunchBankIdApp(BankIdSupportedDevice detectedDevice)
    {
        // On Android, some browsers will (for security reasons) not launching a
        // third party app/scheme (BankID) if there is no user interaction.
        //
        // - Chrome, Edge, Samsung Internet Browser and Brave is confirmed to require User Interaction
        // - Firefox and Opera is confirmed to work without User Interaction

        return detectedDevice.DeviceOs == BankIdSupportedDeviceOs.Android
               && detectedDevice.DeviceBrowser != BankIdSupportedDeviceBrowser.Firefox
               && detectedDevice.DeviceBrowser != BankIdSupportedDeviceBrowser.Opera;
    }

    private bool GetDeviceWillReloadPageOnReturnFromBankIdApp(BankIdSupportedDevice detectedDevice)
    {
        // Allow users to customize reload behaviour.
        if (_reloadPageOnReturnFromBankIdApp != null)
        {
            return _reloadPageOnReturnFromBankIdApp.DeviceWillReloadPageOnReturn(detectedDevice);
        }

        // When returned from the BankID app Safari on iOS will refresh the page/tab.

        return detectedDevice.DeviceOs == BankIdSupportedDeviceOs.Ios
               && detectedDevice.DeviceBrowser == BankIdSupportedDeviceBrowser.Safari;
    }

    private async Task<string> GetLaunchUrl(BankIdSupportedDevice device, LaunchUrlRequest request, List<IBankIdLauncherCustomAppCallback> customAppCallbacks)
    {
        var prefix = GetPrefixPart(device);
        var queryString = await GetQueryStringPart(device, request, customAppCallbacks);

        return $"{prefix}{queryString}";
    }

    private string GetPrefixPart(BankIdSupportedDevice device)
    {
        return CanUseAppLink(device)
            ? BankIdAppLinkPrefix
            : BankIdSchemePrefix;
    }

    private static bool CanUseAppLink(BankIdSupportedDevice device)
    {
        // Only Safari on IOS and Chrome or Edge on Android version >= 6 seems to support
        //  the https://app.bankid.com/ launch url

        return IsSafariOnIos(device)
               || IsChromeOrEdgeOnAndroid6OrGreater(device);
    }

    private static bool IsSafariOnIos(BankIdSupportedDevice device)
    {
        return device.DeviceOs == BankIdSupportedDeviceOs.Ios
               && device.DeviceBrowser == BankIdSupportedDeviceBrowser.Safari;
    }

    private static bool IsChromeOrEdgeOnAndroid6OrGreater(BankIdSupportedDevice device)
    {
        return device.DeviceOs == BankIdSupportedDeviceOs.Android
               && device.DeviceOsVersion.MajorVersion >= 6
               && (
                   device.DeviceBrowser == BankIdSupportedDeviceBrowser.Chrome
                   || device.DeviceBrowser == BankIdSupportedDeviceBrowser.Edge
               );
    }

    private async Task<string> GetQueryStringPart(BankIdSupportedDevice device, LaunchUrlRequest request, List<IBankIdLauncherCustomAppCallback> customAppCallbacks)
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

        var redirectUrl = await GetRedirectUrl(device, request, customAppCallbacks);
        queryStringParams.Add(BankIdRedirectQueryStringParamName, redirectUrl);

        return QueryStringGenerator.ToQueryString(queryStringParams);
    }

    private static async Task<string> GetRedirectUrl(BankIdSupportedDevice device, LaunchUrlRequest request, List<IBankIdLauncherCustomAppCallback> customAppCallbacks)
    {
        // Allow for easy override of callback url
        var customAppCallbackContext = new BankIdLauncherCustomAppCallbackContext(device, request);
        var customAppCallback = await GetRelevantCustomAppCallbackAsync(customAppCallbackContext, customAppCallbacks);
        if (customAppCallback != null)
        {
            return await customAppCallback.GetCustomAppReturnUrl(customAppCallbackContext);
        }

        // Only use redirect url for iOS as recommended in BankID Guidelines 3.1.2
        return device.DeviceOs == BankIdSupportedDeviceOs.Ios
            ? GetIOsBrowserSpecificRedirectUrl(device, request.RedirectUrl)
            : NullRedirectUrl;
    }

    private static string GetIOsBrowserSpecificRedirectUrl(BankIdSupportedDevice device, string redirectUrl)
    {
        // If it is a third party browser, don't specify the return URL, just the browser scheme.
        // This will launch the browser with the last page used (the Active Login status page).
        // If a URL is specified these browsers will open that URL in a new tab and we will lose context.

        return device.DeviceBrowser switch
        {
            // Safari can only be launched by providing redirect url (https://...)
            BankIdSupportedDeviceBrowser.Safari => redirectUrl,

            // Normally you would supply the URL, but we just want to launch the app again
            BankIdSupportedDeviceBrowser.Chrome => IosChromeScheme,
            BankIdSupportedDeviceBrowser.Firefox => IosFirefoxScheme,

            // Opens a new tab on app launch, so can't launch automatically
            BankIdSupportedDeviceBrowser.Edge => string.Empty,
            BankIdSupportedDeviceBrowser.Opera => string.Empty,

            // Return empty string so user can go back manually, will catch unknown third party browsers
            _ => string.Empty
        };
    }

    private static async Task<IBankIdLauncherCustomAppCallback?> GetRelevantCustomAppCallbackAsync(BankIdLauncherCustomAppCallbackContext customAppCallbackContext, List<IBankIdLauncherCustomAppCallback> customAppCallbacks)
    {
        foreach (var callback in customAppCallbacks)
        {
            if (await callback.IsApplicable(customAppCallbackContext))
            {
                return callback;
            }
        }

        return null;
    }

    private static string Base64Encode(string value)
    {
        var encodedBytes = Encoding.Unicode.GetBytes(value);
        return Convert.ToBase64String(encodedBytes);
    }
}
