using System;
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
        private readonly IGrandIdEnviromentConfiguration _enviromentConfiguration;

        /// <summary>
        /// Creates an instance of <see cref="GrandIdApiClient"/> using the supplied <see cref="HttpClient"/> to talk HTTP.
        /// </summary>
        /// <param name="httpClient">The HttpClient to use.</param>
        /// <param name="jsonSerializer">The JsonSerializer to se for serializing and deserializing requests and responses.</param>
        /// <param name="enviromentConfiguration"></param>
        public GrandIdApiClient(HttpClient httpClient, IJsonSerializer jsonSerializer, IGrandIdEnviromentConfiguration enviromentConfiguration)
        {
            _httpClient = httpClient;
            _jsonSerializer = jsonSerializer;
            _enviromentConfiguration = enviromentConfiguration;
        }

        /// <summary>
        /// Request a redirectUrl to be used for authentication against GrandId
        /// </summary>
        /// <returns>If request is successfull returns a sessionId and a redirectUrl. </returns>
        public async Task<AuthResponse> AuthAsync(AuthRequest request)
        {
            //TODO: URL Encode query string parameters
            var authResponse = await _httpClient.GetAsync<AuthResponse>("/FederatedLogin?apiKey=" + _enviromentConfiguration.ApiKey + "&authenticateServiceKey=" + GetDeviceOptionKey(request.DeviceOption) + "&callbackUrl=" + request.CallbackUrl, _jsonSerializer);
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
            //TODO: URL Encode query string parameters
            var sessionResponse = await _httpClient.GetAsync<SessionStateResponse>("/GetSession?apiKey=" + _enviromentConfiguration.ApiKey + "&authenticateServiceKey=" + GetDeviceOptionKey(request.DeviceOption) + "&sessionid=" + request.SessionId, _jsonSerializer);
            //TODO: Wrap error object and don't return if success
            if (sessionResponse.ErrorObject != null)
            {
                throw new GrandIdApiException(sessionResponse.ErrorObject.Code, sessionResponse.ErrorObject.Message);
            }
            return sessionResponse;
        }

        private string GetDeviceOptionKey(DeviceOption deviceOption)
        {
            switch (deviceOption)
            {
                case DeviceOption.SameDevice:
                    return _enviromentConfiguration.SameDeviceServiceKey;
                case DeviceOption.OtherDevice:
                    return _enviromentConfiguration.OtherDeviceServiceKey;
                case DeviceOption.ChooseDevice:
                    return _enviromentConfiguration.ChooseDeviceServiceKey;
                default:
                    throw new ArgumentException("Invalid device option", nameof(deviceOption));
            }
        }
    }
}
