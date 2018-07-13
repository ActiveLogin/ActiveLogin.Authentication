using System.Net.Http;
using System.Threading.Tasks;
using ActiveLogin.Authentication.BankId.Api.Models;

namespace ActiveLogin.Authentication.BankId.Api
{
    public class BankIdApiClient : IBankIdApiClient
    {
        private readonly HttpClient _httpClient;

        public BankIdApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <summary>
        /// Initiates an authentication or signing order. Use the collect method to query the status of the order.
        /// </summary>
        /// <returns>If the request is successful, the OrderRef and AutoStartToken is returned.</returns>
        public Task<AuthResponse> AuthAsync(AuthRequest request)
        {
            return _httpClient.PostAsync<AuthRequest, AuthResponse>("/auth", request);
        }

        /// <summary>
        /// Collects the result of a sign or auth order using the OrderRef as reference.
        /// RP should keep on calling collect every two seconds as long as status indicates pending.
        /// RP must abort if status indicates failed.
        /// </summary>
        /// <returns>The user identity is returned when complete.</returns>
        public Task<CollectResponse> CollectAsync(CollectRequest request)
        {
            return _httpClient.PostAsync<CollectRequest, CollectResponse>("/collect", request);
        }

        /// <summary>
        /// Cancels an ongoing sign or auth order.
        /// This is typically used if the user cancels the order in your service or app.
        /// </summary>
        public Task<CancelResponse> CancelAsync(CancelRequest request)
        {
            return _httpClient.PostAsync<CancelRequest, CancelResponse>("/cancel", request);
        }
    }
}
