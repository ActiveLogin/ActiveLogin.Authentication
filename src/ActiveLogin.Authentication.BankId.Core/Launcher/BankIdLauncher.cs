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
    private readonly List<IBankIdLauncherCustomBrowser> _customBrowsers;

    public BankIdLauncher(IBankIdSupportedDeviceDetector bankIdSupportedDeviceDetector, IEnumerable<IBankIdLauncherCustomBrowser> customBrowsers)
    {
        _bankIdSupportedDeviceDetector = bankIdSupportedDeviceDetector;
        _customBrowsers = customBrowsers.ToList();
    }

    public async Task<BankIdLaunchInfo> GetLaunchInfoAsync(LaunchUrlRequest request)
    {
        var detectedDevice = _bankIdSupportedDeviceDetector.Detect();

        var customBrowserContext = new BankIdLauncherCustomBrowserContext(detectedDevice, request);
        var customBrowser = await GetRelevantCustomAppCallbackAsync(customBrowserContext, _customBrowsers);
        var customBrowserConfig = customBrowser != null ? (await customBrowser.GetCustomAppCallbackResult(customBrowserContext)) : null;

        var deviceMightRequireUserInteractionToLaunch = GetDeviceMightRequireUserInteractionToLaunchBankIdApp(detectedDevice, customBrowserConfig);
        var deviceWillReloadPageOnReturn = GetDeviceWillReloadPageOnReturnFromBankIdApp(detectedDevice, customBrowserConfig);
        var launchUrl = GetLaunchUrl(detectedDevice, request, customBrowserConfig);
        var returnUrl = GetRedirectUrl(detectedDevice, request, customBrowserConfig);

        return new BankIdLaunchInfo(launchUrl, deviceMightRequireUserInteractionToLaunch, deviceWillReloadPageOnReturn, returnUrl);
    }

    private bool GetDeviceMightRequireUserInteractionToLaunchBankIdApp(BankIdSupportedDevice detectedDevice, BankIdLauncherCustomBrowserConfig? customBrowserConfig)
    {
        var userInteractionBehaviour = customBrowserConfig?.BrowserMightRequireUserInteractionToLaunch ?? BrowserMightRequireUserInteractionToLaunch.Default;
        
        return userInteractionBehaviour switch
        {
            BrowserMightRequireUserInteractionToLaunch.Always => true,
            BrowserMightRequireUserInteractionToLaunch.Never => false,

            // Modern recommendation from BankID -> on mobile show fallback button
            // Ref: https://developers.bankid.com/resources/ui-guide-mobile
            _ => (detectedDevice.DeviceOs == BankIdSupportedDeviceOs.Ios
                 || detectedDevice.DeviceOs == BankIdSupportedDeviceOs.Android)
        };
    }

    private bool GetDeviceWillReloadPageOnReturnFromBankIdApp(BankIdSupportedDevice detectedDevice, BankIdLauncherCustomBrowserConfig? customBrowserConfig)
    {
        var reloadBehaviour = customBrowserConfig?.BrowserReloadBehaviourOnReturnFromBankIdApp ?? BrowserReloadBehaviourOnReturnFromBankIdApp.Default;

        return reloadBehaviour switch
        {
            BrowserReloadBehaviourOnReturnFromBankIdApp.Always => true,
            BrowserReloadBehaviourOnReturnFromBankIdApp.Never => false,

            // By default, Safari on iOS will refresh the page/tab when returned from the BankID app
            _ => detectedDevice is
            {
                DeviceOs: BankIdSupportedDeviceOs.Ios,
                DeviceBrowser: BankIdSupportedDeviceBrowser.Safari
            }
        };
    }

    private string GetLaunchUrl(BankIdSupportedDevice device, LaunchUrlRequest request, BankIdLauncherCustomBrowserConfig? customBrowserConfig)
    {
        var prefix = GetPrefixPart(device);
        var queryString = GetQueryStringPart(device, request, customBrowserConfig);

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
        // Universal Links (https://app.bankid.com/) are the recommended approach for mobile devices
        // per BankID documentation: https://developers.bankid.com/getting-started/autostart

        return device.DeviceOs == BankIdSupportedDeviceOs.Ios || device.DeviceOs == BankIdSupportedDeviceOs.Android;
    }

    private string GetQueryStringPart(BankIdSupportedDevice device, LaunchUrlRequest request, BankIdLauncherCustomBrowserConfig? customBrowserConfig)
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

        var redirectUrl = GetRedirectUrl(device, request, customBrowserConfig);

        // DEPRECATED: Setting the return URL here is no longer the recommended approach.
        // The return URL should now be specified in the backend call to BankID when
        // creating the order (auth, sign, or payment).
        // This parameter is kept only for backward compatibility. If both values are set,
        // BankID will use the one provided in the backend call.
        queryStringParams.Add(BankIdRedirectQueryStringParamName, redirectUrl);

        return QueryStringGenerator.ToQueryString(queryStringParams);
    }

    private static string GetRedirectUrl(BankIdSupportedDevice device, LaunchUrlRequest request, BankIdLauncherCustomBrowserConfig? customBrowserConfig)
    {
        // Allow for easy override of callback url
        if (customBrowserConfig != null && customBrowserConfig.ReturnUrl != null)
        {
            return customBrowserConfig.ReturnUrl;
        }

        // Only use redirect url for iOS as recommended in BankID Guidelines 3.1.2
        return device.DeviceOs == BankIdSupportedDeviceOs.Ios
            ? GetIOsBrowserSpecificRedirectUrl(device, request.RedirectUrl, customBrowserConfig)
            : NullRedirectUrl;
    }

    private static string GetIOsBrowserSpecificRedirectUrl(BankIdSupportedDevice device, string redirectUrl, BankIdLauncherCustomBrowserConfig? customBrowserConfig)
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

            BankIdSupportedDeviceBrowser.Edge => NullRedirectUrl,
            BankIdSupportedDeviceBrowser.Opera => NullRedirectUrl,

            _ => NullRedirectUrl
        };
    }

    private static async Task<IBankIdLauncherCustomBrowser?> GetRelevantCustomAppCallbackAsync(BankIdLauncherCustomBrowserContext customBrowserContext, List<IBankIdLauncherCustomBrowser> customAppCallbacks)
    {
        foreach (var callback in customAppCallbacks)
        {
            if (await callback.IsApplicable(customBrowserContext))
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
