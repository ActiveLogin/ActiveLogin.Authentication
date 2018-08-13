using System.Threading.Tasks;
using ActiveLogin.Authentication.GrandId.Api.Models;

namespace ActiveLogin.Authentication.GrandId.Api
{
    /// <summary>
    /// Extensions to enable easier access to common api scenarios.
    /// </summary>
    public static class GrandIdApiClientExtensions
    {
        /// <summary>
        /// Initiates an authentication or signing order. Use the collect method to query the status of the order.
        /// </summary>
        /// <param name="apiClient">The <see cref="IGrandIdApiClient"/> instance.</param>
        /// <param name="endUserIp">
        /// The user IP address as seen by RP.String.IPv4 and IPv6 is allowed.
        /// Note the importance of using the correct IP address.It must be the IP address representing the user agent (the end user device) as seen by the RP.
        /// If there is a proxy for inbound traffic, special considerations may need to be taken to get the correct address.
        /// 
        /// In some use cases the IP address is not available, for instance for voice based services.
        /// In this case, the internal representation of those systems IP address is ok to use.
        /// </param>
        /// <returns>If the request is successful, the OrderRef and AutoStartToken is returned.</returns>
        public static Task<AuthResponse> AuthAsync(this IGrandIdApiClient apiClient, string endUserIp)
        {
            return apiClient.AuthAsync(new AuthRequest(endUserIp));
        }

        /// <summary>
        /// Initiates an authentication or signing order. Use the collect method to query the status of the order.
        /// </summary>
        /// <param name="apiClient">The <see cref="IGrandIdApiClient"/> instance.</param>
        /// <param name="endUserIp">
        /// The user IP address as seen by RP.String.IPv4 and IPv6 is allowed.
        /// Note the importance of using the correct IP address.It must be the IP address representing the user agent (the end user device) as seen by the RP.
        /// If there is a proxy for inbound traffic, special considerations may need to be taken to get the correct address.
        /// 
        /// In some use cases the IP address is not available, for instance for voice based services.
        /// In this case, the internal representation of those systems IP address is ok to use.
        /// </param>
        /// <param name="personalIdentityNumber">
        /// The personal number of the user. 12 digits, century must be included (YYYYMMDDSSSC).
        /// If the personal number is excluded, the client must be started with the AutoStartToken returned in the response.
        /// </param>
        /// <returns>If the request is successful, the OrderRef and AutoStartToken is returned.</returns>
        public static Task<AuthResponse> AuthAsync(this IGrandIdApiClient apiClient, string endUserIp, string personalIdentityNumber)
        {
            return apiClient.AuthAsync(new AuthRequest(endUserIp, personalIdentityNumber));
        }

        /// <summary>
        /// Collects the result of a sign or auth order using the OrderRef as reference.
        /// RP should keep on calling collect every two seconds as long as status indicates pending.
        /// RP must abort if status indicates failed.
        /// </summary>
        /// <param name="apiClient">The <see cref="IGrandIdApiClient"/> instance.</param>
        /// <param name="orderRef">The OrderRef returned from auth or sign.</param>
        /// <returns>The user identity is returned when complete.</returns>
        public static Task<CollectResponse> CollectAsync(this IGrandIdApiClient apiClient, string orderRef)
        {
            return apiClient.CollectAsync(new CollectRequest(orderRef));
        }

        /// <summary>
        /// Cancels an ongoing sign or auth order.
        /// This is typically used if the user cancels the order in your service or app.
        /// </summary>
        /// <param name="apiClient">The <see cref="IGrandIdApiClient"/> instance.</param>
        /// <param name="orderRef">The OrderRef returned from auth or sign.</param>
        public static Task<CancelResponse> CancelAsync(this IGrandIdApiClient apiClient, string orderRef)
        {
            return apiClient.CancelAsync(new CancelRequest(orderRef));
        }
    }
}
