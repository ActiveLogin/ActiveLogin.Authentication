using ActiveLogin.Authentication.GrandId.Api.Models;
using System.Threading.Tasks;

namespace ActiveLogin.Authentication.GrandId.Api
{
    /// <summary>
    /// Extensions to enable easier access to common api scenarios.
    /// </summary>
    public static class GrandIdApiClientExtensions
    {
        public static Task<FederatedLoginResponse> FederatedLoginAsync(this IGrandIdApiClient apiClient, string authenticateServiceKey, string callbackUrl)
        {
            return apiClient.FederatedLoginAsync(new FederatedLoginRequest(authenticateServiceKey, callbackUrl));
        }

        public static Task<FederatedLoginResponse> FederatedLoginAsync(this IGrandIdApiClient apiClient, string authenticateServiceKey, string callbackUrl, string personalIdentityNumber)
        {
            return apiClient.FederatedLoginAsync(new FederatedLoginRequest(authenticateServiceKey, callbackUrl, personalIdentityNumber));
        }

        public static Task<FederatedDirectLoginResponse> FederatedDirectLoginAsync(this IGrandIdApiClient apiClient, string authenticateServiceKey, string username, string password)
        {
            return apiClient.FederatedDirectLoginAsync(new FederatedDirectLoginRequest(authenticateServiceKey, username, password));
        }

        public static Task<SessionStateResponse> GetSessionAsync(this IGrandIdApiClient apiClient, string authenticateServiceKey, string sessionId)
        {
            return apiClient.GetSessionAsync(new SessionStateRequest(authenticateServiceKey, sessionId));
        }
        public static Task<LogoutResponse> LogoutAsync(this IGrandIdApiClient apiClient, string sessionId)
        {
            return apiClient.LogoutAsync(new LogoutRequest(sessionId));
        }
    }
}
