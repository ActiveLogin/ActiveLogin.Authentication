using ActiveLogin.Authentication.GrandId.Api.Models;
using System.Threading.Tasks;

namespace ActiveLogin.Authentication.GrandId.Api
{
    /// <summary>
    /// Extensions to enable easier access to common api scenarios.
    /// </summary>
    public static class GrandIdApiClientExtensions
    {
        public static Task<BankIdFederatedLoginResponse> BankIdFederatedLoginAsync(this IGrandIdApiClient apiClient, string authenticateServiceKey, string callbackUrl = null, bool? useChooseDevice = null, bool? useSameDevice = null, bool? askForPersonalIdentityNumber = null, string personalIdentityNumber = null, bool? requireMobileBankId = null, string customerUrl = null, bool? showGui = null, string signUserVisibleData = null, string signUserNonVisibleData = null)
        {
            return apiClient.BankIdFederatedLoginAsync(new BankIdFederatedLoginRequest(callbackUrl, useChooseDevice, useSameDevice, askForPersonalIdentityNumber, personalIdentityNumber, requireMobileBankId, customerUrl, showGui, signUserVisibleData, signUserNonVisibleData));
        }
        
        public static Task<BankIdGetSessionResponse> BankIdGetSessionAsync(this IGrandIdApiClient apiClient, string sessionId)
        {
            return apiClient.BankIdGetSessionAsync(new BankIdGetSessionRequest(sessionId));
        }


        public static Task<LogoutResponse> LogoutAsync(this IGrandIdApiClient apiClient, string sessionId)
        {
            return apiClient.LogoutAsync(new LogoutRequest(sessionId));
        }
    }
}
