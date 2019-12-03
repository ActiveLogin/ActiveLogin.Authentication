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
        private const string AndroidNullRedirectUrl = "null";

        private const string IosChromeSchemePrefix = "googlechromes://";
        private const string IosEdgeSchemePrefix = "microsoft-edge-https://";
        private const string IosFirefoxSchemePrefix = "firefox://open-url?url=";

        public string GetLaunchUrl(BankIdSupportedDevice device, LaunchUrlRequest request)
        {
            var prefix = GetPrefixPart(device);
            var queryString = GetQueryStringPart(device, request);

            return $"{prefix}{queryString}";
        }

        private string GetPrefixPart(BankIdSupportedDevice device)
        {
            if (device.IsIos)
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
            return device.IsIos
                ? GetIOsBrowserSpecificRedirectUrl(device, request.RedirectUrl)
                : AndroidNullRedirectUrl;
        }

        private static string GetIOsBrowserSpecificRedirectUrl(BankIdSupportedDevice device, string redirectUrl)
        {
            if (device.IsChrome || device.IsEdge)
            {
                var redirectUrlWithoutHttpsScheme = redirectUrl.Substring(8);

                if (device.IsChrome)
                {
                    return IosChromeSchemePrefix + redirectUrlWithoutHttpsScheme;
                }

                if (device.IsEdge)
                {
                    return IosEdgeSchemePrefix + redirectUrlWithoutHttpsScheme;
                }
            }

            if (device.IsFirefox)
            {
                return IosFirefoxSchemePrefix + Uri.EscapeDataString(redirectUrl);
            }

            return redirectUrl;
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
