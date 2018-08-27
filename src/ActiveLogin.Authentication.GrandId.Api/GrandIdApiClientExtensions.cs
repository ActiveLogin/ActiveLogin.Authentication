using ActiveLogin.Authentication.GrandId.Api.Models;
using System.Threading.Tasks;

namespace ActiveLogin.Authentication.GrandId.Api
{
    /// <summary>
    /// Extensions to enable easier access to common api scenarios.
    /// </summary>
    public static class GrandIdApiClientExtensions
    {
        public static Task<AuthResponse> AuthAsync(this IGrandIdApiClient apiClient, DeviceOption deviceOption, string callbackUrl)
        {
            return apiClient.AuthAsync(new AuthRequest(deviceOption, callbackUrl));
        }

        public static Task<SessionStateResponse> GetSessionAsync(this IGrandIdApiClient apiClient, DeviceOption deviceOption, string sessionId)
        {
            return apiClient.GetSessionAsync(new SessionStateRequest(deviceOption, sessionId));
        }
    }
}
