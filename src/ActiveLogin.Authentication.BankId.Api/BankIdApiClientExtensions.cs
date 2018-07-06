using System.Threading.Tasks;
using ActiveLogin.Authentication.BankId.Api.Models;

namespace ActiveLogin.Authentication.BankId.Api
{
    public static class BankIdApiClientExtensions
    {
        public static Task<AuthResponse> AuthAsync(this IBankIdApiClient apiClient, string endUserIp)
        {
            return apiClient.AuthAsync(new AuthRequest(endUserIp));
        }

        public static Task<AuthResponse> AuthAsync(this IBankIdApiClient apiClient, string endUserIp, string personalIdentityNumber)
        {
            return apiClient.AuthAsync(new AuthRequest(endUserIp, personalIdentityNumber));
        }

        public static Task<CollectResponse> CollectAsync(this IBankIdApiClient apiClient, string orderRef)
        {
            return apiClient.CollectAsync(new CollectRequest(orderRef));
        }

        public static Task<CancelResponse> CancelAsync(this IBankIdApiClient apiClient, string orderRef)
        {
            return apiClient.CancelAsync(new CancelRequest(orderRef));
        }
    }
}
