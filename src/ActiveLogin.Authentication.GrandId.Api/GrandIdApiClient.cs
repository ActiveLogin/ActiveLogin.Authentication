﻿using System;
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
        /// <param name="apiKey">ApiKey for GrandId.</param>
        public GrandIdApiClient(HttpClient httpClient, IJsonSerializer jsonSerializer, string apiKey)
        {
            _httpClient = httpClient;
            _jsonSerializer = jsonSerializer;
            _apiKey = apiKey;
        }

        /// <summary>
        /// Request a redirectUrl to be used for authentication against GrandId
        /// </summary>
        /// <returns>If request is successfull returns a sessionId and a redirectUrl. </returns>
        public async Task<AuthResponse> AuthAsync(AuthRequest request)
        {
            var url = GetUrl("/FederatedLogin", new Dictionary<string, string>
            {
                { "apiKey", _apiKey },
                { "authenticateServiceKey", request.AuthenticateServiceKey },
                { "callbackUrl", request.CallbackUrl }
            });
            var authResponse = await _httpClient.GetAsync<AuthResponse>(url, _jsonSerializer);
            //TODO: Wrap error object and don't return if success
            if (authResponse.ErrorObject != null)
            {
                throw new GrandIdApiException(authResponse.ErrorObject.Code, authResponse.ErrorObject.Message);
            }
            return authResponse;
        }

        /// <summary>
        /// Requests the state of a login for a sessionId
        /// </summary>
        /// <returns>If the request is successfull returns the status of the login</returns>
        public async Task<SessionStateResponse> GetSessionAsync(SessionStateRequest request)
        {
            var url = GetUrl("/GetSession", new Dictionary<string, string>
            {
                { "apiKey", _apiKey },
                { "authenticateServiceKey", request.AuthenticateServiceKey },
                { "sessionid", request.SessionId }
            });
            var sessionResponse = await _httpClient.GetAsync<SessionStateResponse>(url, _jsonSerializer);
            //TODO: Wrap error object and don't return if success
            if (sessionResponse.ErrorObject != null)
            {
                throw new GrandIdApiException(sessionResponse.ErrorObject.Code, sessionResponse.ErrorObject.Message);
            }
            return sessionResponse;
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
