using System.Threading.Tasks;
using ActiveLogin.Authentication.GrandId.Api.Models;

namespace ActiveLogin.Authentication.GrandId.Api
{
    /// <summary>
    /// GrandD API Client that defines the supported methods.
    /// </summary>
    public interface IGrandIdApiClient
    {
        /// <summary>
        /// Initiates an authentication chain. Use this method to retrieve the url to redirect the user to.
        /// </summary>
        /// <returns>If the request is successful, the redirectUrl and sessionId is returned</returns>
        Task<FederatedLoginResponse> FederatedLoginAsync(FederatedLoginRequest request);

        /// <summary>
        /// Fetches the currents Session Data for a sessionId.
        /// </summary>
        /// <returns>If the request is successful, the sessionData is returned</returns>
        Task<SessionStateResponse> GetSessionAsync(SessionStateRequest request);
    }
}