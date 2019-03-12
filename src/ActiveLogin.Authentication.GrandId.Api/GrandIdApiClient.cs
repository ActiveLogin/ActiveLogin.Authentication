using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ActiveLogin.Authentication.GrandId.Api.Models;

namespace ActiveLogin.Authentication.GrandId.Api
{
    /// <summary>
    ///     HTTP based client for the GrandId REST API.
    /// </summary>
    public class GrandIdApiClient : IGrandIdApiClient
    {
        private readonly string _apiKey;
        private readonly string _bankIdServiceKey;
        private readonly HttpClient _httpClient;

        public GrandIdApiClient(HttpClient httpClient, GrandIdApiClientConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration.ApiKey;
            _bankIdServiceKey = configuration.BankIdServiceKey;

            if (string.IsNullOrEmpty(configuration.ApiKey))
                throw new InvalidOperationException($"A valid '{nameof(configuration.ApiKey)}' must be provided.'");
        }

        /// <summary>
        ///     This is the function to log in using an apiKey, authenticateServiceKey and a callbackUrl.
        ///     The return value will be a sessionid and a return URL.
        /// </summary>
        /// <returns>If the request is successful, the redirectUrl and sessionId is returned</returns>
        public async Task<BankIdFederatedLoginResponse> BankIdFederatedLoginAsync(BankIdFederatedLoginRequest request)
        {
            EnsureValidBankIdServiceKey();

            var queryStringParams = new Dictionary<string, string>
            {
                { "apiKey", _apiKey },
                { "authenticateServiceKey", _bankIdServiceKey }
            };
            string url = GetUrl("FederatedLogin", queryStringParams);
            var postData = new Dictionary<string, string>
            {
                { "callbackUrl", GetBase64EncodedString(request.CallbackUrl) },
                { "deviceChoice", GetBoolString(request.UseChooseDevice) },
                { "thisDevice", GetBoolString(request.UseSameDevice) },
                { "askForSSN", GetBoolString(request.AskForPersonalIdentityNumber) },
                { "personalNumber", request.PersonalIdentityNumber },
                { "mobileBankId", GetBoolString(request.RequireMobileBankId) },
                { "customerURL", GetBase64EncodedString(request.CustomerUrl) },
                { "gui", GetBoolString(request.ShowGui) },
                { "userVisibleData", GetBase64EncodedString(request.SignUserVisibleData) },
                { "userNonVisibleData", GetBase64EncodedString(request.SignUserNonVisibleData) }
            };

            BankIdFederatedLoginFullResponse fullResponse =
                await PostFullResponseAndEnsureSuccess<BankIdFederatedLoginFullResponse>(url, postData);
            return new BankIdFederatedLoginResponse(fullResponse);
        }

        /// <summary>
        ///     Fetches the currents Session Data for a sessionId.
        /// </summary>
        /// <returns>If the request is successful, the sessionData is returned</returns>
        public async Task<BankIdGetSessionResponse> BankIdGetSessionAsync(BankIdGetSessionRequest request)
        {
            EnsureValidBankIdServiceKey();

            string url = GetUrl("GetSession", new Dictionary<string, string>
            {
                { "apiKey", _apiKey },
                { "authenticateServiceKey", _bankIdServiceKey },
                { "sessionid", request.SessionId }
            });

            BankIdGetSessionFullResponse fullResponse =
                await GetFullResponseAndEnsureSuccess<BankIdGetSessionFullResponse>(url);
            return new BankIdGetSessionResponse(fullResponse);
        }

        /// <summary>
        ///     This is the function to logout a user from an IDP.
        /// </summary>
        public async Task<LogoutResponse> LogoutAsync(LogoutRequest request)
        {
            string url = GetUrl("Logout", new Dictionary<string, string>
            {
                { "apiKey", _apiKey },
                { "sessionid", request.SessionId }
            });

            LogoutFullResponse fullResponse = await GetFullResponseAndEnsureSuccess<LogoutFullResponse>(url);
            return new LogoutResponse(fullResponse);
        }


        private static string GetUrl(string baseUrl, Dictionary<string, string> queryStringParams)
        {
            if (!queryStringParams.Any()) return baseUrl;

            string queryString = string.Join("&",
                queryStringParams.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));
            return $"{baseUrl}?{queryString}";
        }

        private async Task<TResult> GetFullResponseAndEnsureSuccess<TResult>(string url)
            where TResult : FullResponseBase
        {
            TResult fullResponse = await _httpClient.GetAsync<TResult>(url);
            if (fullResponse.ErrorObject != null) throw new GrandIdApiException(fullResponse.ErrorObject);

            return fullResponse;
        }

        private async Task<TResult> PostFullResponseAndEnsureSuccess<TResult>(string url,
            Dictionary<string, string> postData) where TResult : FullResponseBase
        {
            Dictionary<string, string> postDataWithoutNullValues =
                postData.Where(pair => pair.Value != null).ToDictionary(x => x.Key, x => x.Value);
            TResult fullResponse = await _httpClient.PostAsync<TResult>(url, postDataWithoutNullValues);
            if (fullResponse.ErrorObject != null) throw new GrandIdApiException(fullResponse.ErrorObject);

            return fullResponse;
        }

        private static string GetBase64EncodedString(string value)
        {
            if (value == null) return null;

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
        }

        private static string GetBoolString(bool? value)
        {
            if (value == null) return null;

            return value.Value ? bool.TrueString.ToLower() : bool.FalseString.ToLower();
        }

        private void EnsureValidBankIdServiceKey()
        {
            if (string.IsNullOrEmpty(_bankIdServiceKey))
                throw new InvalidOperationException("A valid 'bankIdServiceKey' must be provided.'");
        }
    }
}
