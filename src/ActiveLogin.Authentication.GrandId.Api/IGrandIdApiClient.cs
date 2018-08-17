using System.Threading.Tasks;
using ActiveLogin.Authentication.GrandId.Api.Models;

namespace ActiveLogin.Authentication.GrandId.Api
{
    /// <summary>
    /// BankID API Client that defines the supported methods as defined in the document "BankID Relying Party Guidelines".
    /// </summary>
    public interface IGrandIdApiClient
    {
        /// <summary>
        /// Initiates an authentication chain. Use this method to retrieve the url to redirect the user to.
        /// </summary>
        /// <returns>If the request is successful, the redirectUrl and sessionId is returned</returns>
        Task<AuthResponse> AuthAsync(AuthRequest request);

        /// <summary>
        /// Fetches the currents Session Data for a sessionId
        /// </summary>
        /// <returns>If the request is successful, the sessionData is returned</returns>
        Task<SessionStateResponse> GetSessionAsync(SessionStateRequest request);
    }
}