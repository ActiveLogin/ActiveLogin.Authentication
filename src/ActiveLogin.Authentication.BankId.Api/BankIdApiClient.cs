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

        public Task<AuthResponse> AuthAsync(AuthRequest request)
        {
            return _httpClient.PostAsync<AuthRequest, AuthResponse>("/auth", request);
        }

        public Task<CollectResponse> CollectAsync(CollectRequest request)
        {
            return _httpClient.PostAsync<CollectRequest, CollectResponse>("/collect", request);
        }

        public Task<CancelResponse> CancelAsync(CancelRequest request)
        {
            return _httpClient.PostAsync<CancelRequest, CancelResponse>("/cancel", request);
        }
    }
}
