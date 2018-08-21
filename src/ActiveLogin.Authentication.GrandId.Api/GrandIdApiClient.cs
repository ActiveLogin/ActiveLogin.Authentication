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


        public async Task<AuthResponse> AuthAsync(AuthRequest request)
        {
            var apiKey = EnviromentConfiguration.ApiKey;
            var deviceOptionKey =  EnviromentConfiguration.GetDeviceOptionKey(request.DeviceOption);
            var authResponse = await _httpClient.GetAsync<AuthResponse>("/FederatedLogin?apiKey=" + apiKey + "&authenticateServiceKey=" + deviceOptionKey + "&callbackUrl=" + request.CallbackUrl, _jsonSerializer);
            if (authResponse.ErrorObject != null)
            {
                throw new GrandIdApiException(authResponse.ErrorObject.Code, authResponse.ErrorObject.Message);
            }
            return authResponse;
        }

        public async Task<SessionStateResponse> GetSessionAsync(SessionStateRequest request)
        {
            var apiKey = EnviromentConfiguration.ApiKey; 
            var deviceOptionKey = EnviromentConfiguration.GetDeviceOptionKey(request.DeviceOption);
            var sessionResponse = await _httpClient.GetAsync<SessionStateResponse>("/GetSession?apiKey=" + apiKey + "&authenticateServiceKey=" + deviceOptionKey + "&sessionid=" + request.SessionId, _jsonSerializer);
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
