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
        /// This is the function to log in using an apiKey, authenticateServiceKey and a callbackUrl.
        /// The return value will be a sessionid and a return URL.
        /// </summary>
        /// <returns>If the request is successful, the redirectUrl and sessionId is returned</returns>
        Task<FederatedLoginResponse> FederatedLoginAsync(FederatedLoginRequest request);

        /// <summary>
        /// This is the function for logging in using an apiKey, authenticateServiceKey, username and password.
        /// The value returned value will be the user’s properties.
        /// </summary>
        /// <returns>If the request is successful, the redirectUrl and sessionId is returned</returns>
        Task<FederatedDirectLoginResponse> FederatedDirectLoginAsync(FederatedDirectLoginRequest request);

        /// <summary>
        /// Fetches the currents Session Data for a sessionId.
        /// </summary>
        /// <returns>If the request is successful, the sessionData is returned</returns>
        Task<SessionStateResponse> GetSessionAsync(SessionStateRequest request);

        /// <summary>
        /// This is the function to logout a user from an IDP.
        /// </summary>
        Task<LogoutResponse> LogoutAsync(LogoutRequest request);
    }
}