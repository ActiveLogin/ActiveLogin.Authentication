using System.Net.Http;
using System.Threading.Tasks;
using ActiveLogin.Authentication.Common.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api
{
    /// <summary>
    /// HTTP based client for the GrandId REST API.
    /// </summary>
    public class GrandIdApiClient : IGrandIdApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IJsonSerializer _jsonSerializer;
        private IGrandIdEnviromentConfiguration EnviromentConfiguration;
        /// <summary>
        /// Creates an instance of <see cref="GrandIdApiClient"/> using the supplied <see cref="HttpClient"/> to talk HTTP.
        /// </summary>
        /// <param name="httpClient">The HttpClient to use.</param>
        /// <param name="jsonSerializer">The JsonSerializer to se for serializing and deserializing requests and responses.</param>
        public GrandIdApiClient(HttpClient httpClient, IJsonSerializer jsonSerializer)
        {
            _httpClient = httpClient;
            _jsonSerializer = jsonSerializer;
        }
        /// <summary>
        /// Request a redirectUrl to be used for authentication against GrandId
        /// </summary>
        /// <returns>If request is successfull returns a sessionId and a redirectUrl. </returns>
        public async Task<AuthResponse> AuthAsync(AuthRequest request)
        {
            var authResponse = await _httpClient.GetAsync<AuthResponse>("/FederatedLogin?apiKey=" + EnviromentConfiguration.ApiKey + "&authenticateServiceKey=" + EnviromentConfiguration.GetDeviceOptionKey(request.DeviceOption) + "&callbackUrl=" + request.CallbackUrl, _jsonSerializer);
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
            var sessionResponse = await _httpClient.GetAsync<SessionStateResponse>("/GetSession?apiKey=" + EnviromentConfiguration.ApiKey + "&authenticateServiceKey=" + EnviromentConfiguration.GetDeviceOptionKey(request.DeviceOption) + "&sessionid=" + request.SessionId, _jsonSerializer);
            if (sessionResponse.ErrorObject != null)
            {
                throw new GrandIdApiException(sessionResponse.ErrorObject.Code, sessionResponse.ErrorObject.Message);
            }
            return sessionResponse;
        }

        public void SetConfiguration(IGrandIdEnviromentConfiguration configuration)
        {
            EnviromentConfiguration = configuration;
        }
    }
}
