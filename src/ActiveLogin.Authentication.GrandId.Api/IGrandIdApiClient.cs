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
        /// Login using GrandID BankID.
        /// 
        /// This is the function to log in using an apiKey, authenticateServiceKey and a callbackUrl.
        /// The return value will be a sessionid and a return URL.
        /// </summary>
        /// <returns>If the request is successful, the redirectUrl and sessionId is returned</returns>
        Task<BankIdFederatedLoginResponse> BankIdFederatedLoginAsync(BankIdFederatedLoginRequest request);

        /// <summary>
        /// Fetches the currents Session Data for a BankID sessionId.
        /// </summary>
        /// <returns>If the request is successful, the sessionData is returned</returns>
        Task<BankIdSessionStateResponse> BankIdGetSessionAsync(BankIdSessionStateRequest request);


        /// <summary>
        /// This is the function for logging in using an apiKey, authenticateServiceKey, username and password.
        /// The value returned value will be the user’s properties.
        /// </summary>
        /// <returns>If the request is successful, the redirectUrl and sessionId is returned</returns>
        Task<FederatedDirectLoginResponse> FederatedDirectLoginAsync(FederatedDirectLoginRequest request);


        /// <summary>
        /// This is the function to logout a user from an IDP.
        /// </summary>
        Task<LogoutResponse> LogoutAsync(LogoutRequest request);
    }
}