using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActiveLogin.Authentication.BankId.AspNetCore.UserMessage;
using Microsoft.AspNetCore.Http.Extensions;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public class BankIdLauncher : IBankIdLauncher
    {
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
                return "https://app.bankid.com/";
            }

            return "bankid:///";
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

            // Don't set any redirect url on Desktop as it opens in a new tab
            var redirectUrl = device.IsDesktop ? "null" : request.RedirectUrl;
            queryStringParams.Add("redirect", redirectUrl);

            return GetQueryString(queryStringParams);
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