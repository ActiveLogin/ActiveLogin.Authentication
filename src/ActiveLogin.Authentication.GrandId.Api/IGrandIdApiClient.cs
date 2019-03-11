using System.Threading.Tasks;
using ActiveLogin.Authentication.GrandId.Api.Models;

namespace ActiveLogin.Authentication.GrandId.Api
{
    /// <summary>
    ///     GrandD API Client that defines the supported methods.
    /// </summary>
    public interface IGrandIdApiClient
    {
        /// <summary>
        ///     Login using GrandID BankID.
        ///     This is the function to log in using an apiKey, authenticateServiceKey and a callbackUrl.
        ///     The return value will be a sessionid and a return URL.
        /// </summary>
        /// <returns>If the request is successful, the redirectUrl and sessionId is returned</returns>
        Task<BankIdFederatedLoginResponse> BankIdFederatedLoginAsync(BankIdFederatedLoginRequest request);

        /// <summary>
        ///     Fetches the currents Session Data for a BankID sessionId.
        /// </summary>
        /// <returns>If the request is successful, the sessionData is returned</returns>
        Task<BankIdGetSessionResponse> BankIdGetSessionAsync(BankIdGetSessionRequest request);


        /// <summary>
        ///     This is the function to logout a user from an IDP.
        /// </summary>
        Task<LogoutResponse> LogoutAsync(LogoutRequest request);
    }
}
