using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice;
using Microsoft.AspNetCore.Http.Extensions;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Launcher
{
    internal class BankIdLauncher : IBankIdLauncher
    {
        private const string BankIdScheme = "bankid:///";

        private const string IosUrlPrefix = "https://app.bankid.com/";
        private const string NullRedirectUrl = "null";

        private const string IosChromeScheme = "googlechromes://";
        private const string IosFirefoxScheme = "firefox://";

        public string GetLaunchUrl(BankIdSupportedDevice device, LaunchUrlRequest request)
        {
            var prefix = GetPrefixPart(device);
            var queryString = GetQueryStringPart(device, request);

            return $"{prefix}{queryString}";
        }

        private string GetPrefixPart(BankIdSupportedDevice device)
        {
            // Only Safari on IOS seems to support the app.bankid.com reference
            if (device.DeviceOs == BankIdSupportedDeviceOs.Ios
                && device.DeviceBrowser == BankIdSupportedDeviceBrowser.Safari)
            {
                return IosUrlPrefix;
            }

            return BankIdScheme;
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

                _ => string.Empty // Return empty string so user can go back manually, will catch Edge, other third party browsers and PWAs
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
