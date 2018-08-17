using System.Net.Http;
using System.Threading.Tasks;
using ActiveLogin.Authentication.GrandId.Api.Models;
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


        public Task<AuthResponse> AuthAsync(AuthRequest request)
        {

            return _httpClient.getAsync<AuthRequest, AuthResponse>("/FederatedLogin?apiKey=" + request.ApiKey + "&authenticateServiceKey=" + request.AuthenticateServiceKey + "&callbackUrl=" + request.CallbackUrl, request, _jsonSerializer);
            //return _httpClient.PostAsync<AuthRequest, AuthResponse>("/FederatedLogin", request, _jsonSerializer);
        }

        public Task<SessionStateResponse> GetSessionAsync(SessionStateRequest request)
        {
            return _httpClient.getAsync<SessionStateRequest, SessionStateResponse>("/GetSession?apiKey=" + request.ApiKey + "&authenticateServiceKey=" + request.AuthenticateServiceKey + "&sessionid=" + request.SessionId, request, _jsonSerializer);

            //return _httpClient.PostAsync<SessionStateRequest, SessionStateResponse>("/GetSession", request, _jsonSerializer);
        }
    }
}
