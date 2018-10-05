using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ActiveLogin.Authentication.Common.Serialization;
using ActiveLogin.Authentication.GrandId.Api.Models;

namespace ActiveLogin.Authentication.GrandId.Api
{
    /// <summary>
    /// HTTP based client for the GrandId REST API.
    /// </summary>
    public class GrandIdApiClient : IGrandIdApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly string _apiKey;

        public GrandIdApiClient(HttpClient httpClient, IJsonSerializer jsonSerializer, GrandIdApiClientConfiguration configuration)
        {
            _httpClient = httpClient;
            _jsonSerializer = jsonSerializer;
            _apiKey = configuration.ApiKey;
        }

        /// <summary>
        /// This is the function to log in using an apiKey, authenticateServiceKey and a callbackUrl.
        /// The return value will be a sessionid and a return URL.
        /// </summary>
        /// <returns>If the request is successful, the redirectUrl and sessionId is returned</returns>
        public async Task<FederatedLoginResponse> FederatedLoginAsync(FederatedLoginRequest request)
        {
            var queryStringParams = new Dictionary<string, string>
            {
                { "apiKey", _apiKey },
                { "authenticateServiceKey", request.AuthenticateServiceKey },
                { "callbackUrl", request.CallbackUrl }
            };

            if (!string.IsNullOrEmpty(request.PersonalIdentityNumber))
            {
                queryStringParams.Add("pnr", request.PersonalIdentityNumber);
            }

            var url = GetUrl("FederatedLogin", queryStringParams);

            var fullResponse = await _httpClient.GetAsync<FederatedLoginFullResponse>(url, _jsonSerializer);
            if (fullResponse.ErrorObject != null)
            {
                throw new GrandIdApiException(fullResponse.ErrorObject.Code, fullResponse.ErrorObject.Message);
            }

            return new FederatedLoginResponse(fullResponse);
        }

        /// <summary>
        /// This is the function for logging in using an apiKey, authenticateServiceKey, username and password.
        /// The value returned value will be the user’s properties.
        /// </summary>
        /// <returns>If the request is successful, the redirectUrl and sessionId is returned</returns>
        public async Task<FederatedDirectLoginResponse> FederatedDirectLoginAsync(FederatedDirectLoginRequest request)
        {
            var url = GetUrl("FederatedDirectLogin", new Dictionary<string, string>
            {
                { "apiKey", _apiKey },
                { "authenticateServiceKey", request.AuthenticateServiceKey },
                { "username", request.Username },
                { "password", request.Password }
            });

            var fullResponse = await _httpClient.GetAsync<FederatedDirectLoginFullResponse>(url, _jsonSerializer);
            if (fullResponse.ErrorObject != null)
            {
                throw new GrandIdApiException(fullResponse.ErrorObject.Code, fullResponse.ErrorObject.Message);
            }

            return new FederatedDirectLoginResponse(fullResponse);
        }

        /// <summary>
        /// Fetches the currents Session Data for a sessionId.
        /// </summary>
        /// <returns>If the request is successful, the sessionData is returned</returns>
        public async Task<SessionStateResponse> GetSessionAsync(SessionStateRequest request)
        {
            var url = GetUrl("GetSession", new Dictionary<string, string>
            {
                { "apiKey", _apiKey },
                { "authenticateServiceKey", request.AuthenticateServiceKey },
                { "sessionid", request.SessionId }
            });

            var fullResponse = await _httpClient.GetAsync<SessionStateFullResponse>(url, _jsonSerializer);
            if (fullResponse.ErrorObject != null)
            {
                throw new GrandIdApiException(fullResponse.ErrorObject.Code, fullResponse.ErrorObject.Message);
            }

            return new SessionStateResponse(fullResponse);
        }

        /// <summary>
        /// This is the function to logout a user from an IDP.
        /// </summary>
        public async Task<LogoutResponse> LogoutAsync(LogoutRequest request)
        {
            var url = GetUrl("Logout", new Dictionary<string, string>
            {
                { "apiKey", _apiKey },
                { "sessionid", request.SessionId }
            });

            var fullResponse = await _httpClient.GetAsync<LogoutFullResponse>(url, _jsonSerializer);
            if (fullResponse.ErrorObject != null)
            {
                throw new GrandIdApiException(fullResponse.ErrorObject.Code, fullResponse.ErrorObject.Message);
            }

            return new LogoutResponse(fullResponse);
        }

        internal static string GetUrl(string baseUrl, Dictionary<string, string> queryStringParams)
        {
            if (!queryStringParams.Any())
            {
                return baseUrl;
            }

            var queryString = string.Join("&", queryStringParams.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));
            return $"{baseUrl}?{queryString}";
        }
    }
}