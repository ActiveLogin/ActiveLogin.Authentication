using System.Net.Http;
using System.Threading.Tasks;
using ActiveLogin.Authentication.GrandId.Api.Models;
using ActiveLogin.Authentication.Common.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api
{
    /// <summary>
    /// HTTP based client for the BankID REST API.
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

        /// <summary>
        /// Initiates an authentication or signing order. Use the collect method to query the status of the order.
        /// </summary>
        /// <returns>If the request is successful, the OrderRef and AutoStartToken is returned.</returns>
        public Task<AuthResponse> AuthAsync(AuthRequest request)
        {
            return _httpClient.PostAsync<AuthRequest, AuthResponse>("/auth", request, _jsonSerializer);
        }

        /// <summary>
        /// Collects the result of a sign or auth order using the OrderRef as reference.
        /// RP should keep on calling collect every two seconds as long as status indicates pending.
        /// RP must abort if status indicates failed.
        /// </summary>
        /// <returns>The user identity is returned when complete.</returns>
        public Task<CollectResponse> CollectAsync(CollectRequest request)
        {
            return _httpClient.PostAsync<CollectRequest, CollectResponse>("/collect", request, _jsonSerializer);
        }

        /// <summary>
        /// Cancels an ongoing sign or auth order.
        /// This is typically used if the user cancels the order in your service or app.
        /// </summary>
        public Task<CancelResponse> CancelAsync(CancelRequest request)
        {
            return _httpClient.PostAsync<CancelRequest, CancelResponse>("/cancel", request, _jsonSerializer);
        }
    }
}
