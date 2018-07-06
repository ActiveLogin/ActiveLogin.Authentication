using System;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using ActiveLogin.Authentication.BankId.Api.Models;

namespace ActiveLogin.Authentication.BankId.Api
{
    public class BankIdApiClient : IBankIdApiClient
    {
        private readonly HttpClient _httpClient;

        public BankIdApiClient(X509Certificate2 clientCertificate)
            : this(clientCertificate, BankIdUrls.ProdApiBaseUrl)
        {

        }

        public BankIdApiClient(X509Certificate2 clientCertificate, Uri baseUrl)
        {
            var httpClientHandler = GetHttpClientHandler(clientCertificate);

            _httpClient = new HttpClient(httpClientHandler)
            {
                BaseAddress = baseUrl
            };
        }

        public BankIdApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private static HttpClientHandler GetHttpClientHandler(X509Certificate2 clientCertficiate)
        {
            var handler = new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                SslProtocols = SslProtocols.Tls12
            };

            handler.ClientCertificates.Add(clientCertficiate);

            return handler;
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
