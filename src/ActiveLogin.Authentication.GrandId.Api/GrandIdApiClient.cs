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

        /// <summary>
        /// Creates an instance of <see cref="GrandIdApiClient"/> using the supplied <see cref="HttpClient"/> to talk HTTP.
        /// </summary>
        /// <param name="httpClient">The HttpClient to use.</param>
        /// <param name="jsonSerializer">The JsonSerializer to se for serializing and deserializing requests and responses.</param>
        /// <param name="configuration">Configuration from GrandID.</param>
        public GrandIdApiClient(HttpClient httpClient, IJsonSerializer jsonSerializer, GrandIdApiClientConfiguration configuration)
        {
            _httpClient = httpClient;
            _jsonSerializer = jsonSerializer;
            _apiKey = configuration.ApiKey;
        }

        /// <summary>
        /// Request a redirectUrl to be used for authentication against GrandId
        /// </summary>
        /// <returns>If request is successfull returns a sessionId and a redirectUrl. </returns>
        public async Task<AuthResponse> AuthAsync(AuthRequest request)
        {
            var url = GetUrl("FederatedLogin", new Dictionary<string, string>
            {
                { "apiKey", _apiKey },
                { "authenticateServiceKey", request.AuthenticateServiceKey },
                { "callbackUrl", request.CallbackUrl }
            });

            var fullResponse = await _httpClient.GetAsync<AuthFullResponse>(url, _jsonSerializer);
            if (fullResponse.ErrorObject != null)
            {
                throw new GrandIdApiException(fullResponse.ErrorObject.Code, fullResponse.ErrorObject.Message);
            }

            return new AuthResponse(fullResponse);
        }

        /// <summary>
        /// Requests the state of a login for a sessionId
        /// </summary>
        /// <returns>If the request is successfull returns the status of the login</returns>
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
        
        internal static string GetUrl(string baseUrl, Dictionary<string, string> filter)
        {
            if (!filter.Any())
            {
                return baseUrl;
            }

            var queryString = string.Join("&", filter.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));
            return $"{baseUrl}?{queryString}";
        }
    }
}