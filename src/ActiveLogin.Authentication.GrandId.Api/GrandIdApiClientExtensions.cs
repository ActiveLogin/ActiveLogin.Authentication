using ActiveLogin.Authentication.GrandId.Api.Models;
using System.Threading.Tasks;

namespace ActiveLogin.Authentication.GrandId.Api
{
    /// <summary>
    /// Extensions to enable easier access to common api scenarios.
    /// </summary>
    public static class GrandIdApiClientExtensions
    {
        public static Task<BankIdFederatedLoginResponse> BankIdFederatedLoginAsync(this IGrandIdApiClient apiClient, string authenticateServiceKey, string callbackUrl)
        {
            return apiClient.BankIdFederatedLoginAsync(new BankIdFederatedLoginRequest(authenticateServiceKey, callbackUrl));
        }

        public static Task<BankIdFederatedLoginResponse> BankIdFederatedLoginAsync(this IGrandIdApiClient apiClient, string authenticateServiceKey, string callbackUrl, string personalIdentityNumber)
        {
            return apiClient.BankIdFederatedLoginAsync(new BankIdFederatedLoginRequest(authenticateServiceKey, callbackUrl, personalIdentityNumber));
        }

        public static Task<BankIdSessionStateResponse> BankIdGetSessionAsync(this IGrandIdApiClient apiClient, string authenticateServiceKey, string sessionId)
        {
            return apiClient.BankIdGetSessionAsync(new BankIdSessionStateRequest(authenticateServiceKey, sessionId));
        }


        public static Task<DirectFederatedLoginResponse> DirectFederatedLoginAsync(this IGrandIdApiClient apiClient, string authenticateServiceKey, string username, string password)
        {
            return apiClient.DirectFederatedLoginAsync(new DirectFederatedLoginRequest(authenticateServiceKey, username, password));
        }


        public static Task<LogoutResponse> LogoutAsync(this IGrandIdApiClient apiClient, string sessionId)
        {
            return apiClient.LogoutAsync(new LogoutRequest(sessionId));
        }
    }
}
