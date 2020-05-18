using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Launcher
{
    internal class BankIdLauncher : IBankIdLauncher
    {
        private const string BankIdSchemePrefix = "bankid:///";
        private const string BankIdAppLinkPrefix = "https://app.bankid.com/";

        private const string NullRedirectUrl = "null";

        private const string IosChromeScheme = "googlechromes://";
        private const string IosFirefoxScheme = "firefox://";
        private const string UserAgentHeaderName = "User-Agent";

        private readonly IBankIdSupportedDeviceDetector _bankIdSupportedDeviceDetector;

        public BankIdLauncher(IBankIdSupportedDeviceDetector bankIdSupportedDeviceDetector)
        {
            _bankIdSupportedDeviceDetector = bankIdSupportedDeviceDetector;
        }

        public BankIdLaunchInfo GetLaunchInfo(LaunchUrlRequest request, HttpContext httpContext)
        {
            var detectedDevice = _bankIdSupportedDeviceDetector.Detect(httpContext.Request.Headers[UserAgentHeaderName]);
            var deviceMightRequireUserInteractionToLaunch = GetDeviceMightRequireUserInteractionToLaunchBankIdApp(detectedDevice);
            var deviceWillReloadPageOnReturn = GetDeviceWillReloadPageOnReturnFromBankIdApp(detectedDevice);

            var launchUrl = GetLaunchUrl(detectedDevice, request);
            return new BankIdLaunchInfo(launchUrl, deviceMightRequireUserInteractionToLaunch, deviceWillReloadPageOnReturn);
        }

        private bool GetDeviceMightRequireUserInteractionToLaunchBankIdApp(BankIdSupportedDevice detectedDevice)
        {
            // On Android, some browsers will (for security reasons) not launching a
            // third party app/scheme (BankID) if there is no user interaction.
            //
            // - Chrome and Samsung Internet Browser is confirmed to require User Interaction.
            // - Firefox is confirmed to work without.
            //
            // HTML files to try this out is available under /docs/tests

            return detectedDevice.DeviceOs == BankIdSupportedDeviceOs.Android
                   && detectedDevice.DeviceBrowser != BankIdSupportedDeviceBrowser.Firefox;
        }

        private bool GetDeviceWillReloadPageOnReturnFromBankIdApp(BankIdSupportedDevice detectedDevice)
        {
            // When returned from the BankID app Safari on iOS will refresh the page/tab.

            return detectedDevice.DeviceOs == BankIdSupportedDeviceOs.Ios
                   && detectedDevice.DeviceBrowser == BankIdSupportedDeviceBrowser.Safari;
        }

        private string GetLaunchUrl(BankIdSupportedDevice device, LaunchUrlRequest request)
        {
            var prefix = GetPrefixPart(device);
            var queryString = GetQueryStringPart(device, request);

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
            // Only Safari on IOS and Chrome on Android version >= 6 seems to support
            //  the https://app.bankid.com/ launch url

            return IsSafariOnIos(device) || IsChromeOnAndroid6OrGreater(device);
        }

        private static bool IsSafariOnIos(BankIdSupportedDevice device)
        {
            return device.DeviceOs == BankIdSupportedDeviceOs.Ios
                   && device.DeviceBrowser == BankIdSupportedDeviceBrowser.Safari;
        }

        private static bool IsChromeOnAndroid6OrGreater(BankIdSupportedDevice device)
        {
            return device.DeviceOs == BankIdSupportedDeviceOs.Android
                   && device.DeviceOsVersion.MajorVersion >= 6
                   && device.DeviceBrowser == BankIdSupportedDeviceBrowser.Chrome;
        }

        private string GetQueryStringPart(BankIdSupportedDevice device, LaunchUrlRequest request)
        {
            var queryStringParams = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(request.AutoStartToken))
            {
                queryStringParams.Add("autostarttoken", request.AutoStartToken);
            }

            if (!string.IsNullOrWhiteSpace(request.RelyingPartyReference))
            {
                queryStringParams.Add("rpref", Base64Encode(request.RelyingPartyReference));
            }

            queryStringParams.Add("redirect", GetRedirectUrl(device, request));

            return GetQueryString(queryStringParams);
        }

        private static string GetRedirectUrl(BankIdSupportedDevice device, LaunchUrlRequest request)
        {
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
                BankIdSupportedDeviceBrowser.Chrome => IosChromeScheme,
                BankIdSupportedDeviceBrowser.Firefox => IosFirefoxScheme,
                BankIdSupportedDeviceBrowser.Safari => redirectUrl,

                _ => string.Empty // Return empty string so user can go back manually, will catch Edge and other third party browsers
            };
        }

        private static string Base64Encode(string value)
        {
            var encodedBytes = Encoding.Unicode.GetBytes(value);
            return Convert.ToBase64String(encodedBytes);
        }

        private static string GetQueryString(Dictionary<string, string> queryStringParams)
        {
            if (!queryStringParams.Any())
            {
                return string.Empty;
            }

            var queryBuilder = new QueryBuilder(queryStringParams);
            return queryBuilder.ToQueryString().ToString();
        }
    }
}
