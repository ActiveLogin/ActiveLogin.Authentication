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
        public string GetLaunchUrl(BankIdSupportedDevice device, LaunchUrlRequest request)
        {
            string prefix = GetPrefixPart(device);
            string queryString = GetQueryStringPart(device, request);

            return $"{prefix}{queryString}";
        }

        private string GetPrefixPart(BankIdSupportedDevice device)
        {
            return device.IsIos
                ? "https://app.bankid.com/"
                : "bankid:///";
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
                ? request.RedirectUrl
                : "null";
        }

        private static string Base64Encode(string value)
        {
            byte[] encodedBytes = Encoding.Unicode.GetBytes(value);
            return Convert.ToBase64String(encodedBytes);
        }

        private static string GetQueryString(Dictionary<string, string> queryStringParams)
        {
            return !queryStringParams.Any()
                ? string.Empty
                : new QueryBuilder(queryStringParams)
                    .ToQueryString()
                    .ToString();
        }
    }
}
