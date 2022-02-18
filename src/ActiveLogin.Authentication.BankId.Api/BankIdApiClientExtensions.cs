using System.Threading.Tasks;
using ActiveLogin.Authentication.BankId.Api.Models;

namespace ActiveLogin.Authentication.BankId.Api
{
    /// <summary>
    /// Extensions to enable easier access to common api scenarios.
    /// </summary>
    public static class BankIdApiClientExtensions
    {
        /// <summary>
        /// Initiates an authentication order. Use the collect method to query the status of the order.
        /// </summary>
        /// <param name="apiClient">The <see cref="IBankIdApiClient"/> instance.</param>
        /// <param name="endUserIp">
        /// The user IP address as seen by RP.String.IPv4 and IPv6 is allowed.
        /// Note the importance of using the correct IP address.It must be the IP address representing the user agent (the end user device) as seen by the RP.
        /// If there is a proxy for inbound traffic, special considerations may need to be taken to get the correct address.
        ///
        /// In some use cases the IP address is not available, for instance for voice based services.
        /// In this case, the internal representation of those systems IP address is ok to use.
        /// </param>
        /// <returns>If the request is successful, the OrderRef and AutoStartToken is returned.</returns>
        public static Task<AuthResponse> AuthAsync(this IBankIdApiClient apiClient, string endUserIp)
        {
            return apiClient.AuthAsync(new AuthRequest(endUserIp));
        }

        /// <summary>
        /// Initiates an authentication order. Use the collect method to query the status of the order.
        /// </summary>
        /// <param name="apiClient">The <see cref="IBankIdApiClient"/> instance.</param>
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
        public static Task<AuthResponse> AuthAsync(this IBankIdApiClient apiClient, string endUserIp, string personalIdentityNumber)
        {
            return apiClient.AuthAsync(new AuthRequest(endUserIp, personalIdentityNumber));
        }

        /// <summary>
        /// Initiates an authentication order. Use the collect method to query the status of the order.
        /// </summary>
        /// <param name="apiClient">The <see cref="IBankIdApiClient"/> instance.</param>
        /// <param name="endUserIp">
        /// The user IP address as seen by RP.String.IPv4 and IPv6 is allowed.
        /// Note the importance of using the correct IP address.It must be the IP address representing the user agent (the end user device) as seen by the RP.
        /// If there is a proxy for inbound traffic, special considerations may need to be taken to get the correct address.
        ///
        /// In some use cases the IP address is not available, for instance for voice based services.
        /// In this case, the internal representation of those systems IP address is ok to use.
        /// </param>
        /// <param name="userVisibleData">
        /// A text that is displayed to the user during authentication with BankID, with the
        /// purpose of providing context for the authentication and to enable users to notice if
        /// there is something wrong about the identification and avoid attempted frauds.The
        /// text can be formatted using CR, LF and CRLF for new lines.The text must be
        /// encoded as UTF-8 and then base 64 encoded. 1—1 500 characters after base 64encoding.
        /// </param>
        /// <param name="userVisibleDataFormat">
        /// If present, and set to "simpleMarkdownV1", this parameter indicates that userVisibleData holds formatting characters which, if used correctly, will make the text displayed with the user nicer to look at.
        /// For further information of formatting options, please study the document Guidelines for Formatted Text.
        /// </param>
        /// <returns>If the request is successful, the OrderRef and AutoStartToken is returned.</returns>
        public static Task<AuthResponse> AuthAsync(this IBankIdApiClient apiClient, string endUserIp, string userVisibleData, string userVisibleDataFormat)
        {
            return apiClient.AuthAsync(new AuthRequest(endUserIp, null, null, userVisibleData, null, userVisibleDataFormat));
        }

        /// <summary>
        /// Initiates an authentication order. Use the collect method to query the status of the order.
        /// </summary>
        /// <param name="apiClient">The <see cref="IBankIdApiClient"/> instance.</param>
        /// <param name="endUserIp">
        /// The user IP address as seen by RP.String.IPv4 and IPv6 is allowed.
        /// Note the importance of using the correct IP address.It must be the IP address representing the user agent (the end user device) as seen by the RP.
        /// If there is a proxy for inbound traffic, special considerations may need to be taken to get the correct address.
        ///
        /// In some use cases the IP address is not available, for instance for voice based services.
        /// In this case, the internal representation of those systems IP address is ok to use.
        /// </param>
        /// <param name="userVisibleData">
        /// The text to be displayed and signed. The text can be formatted using CR, LF and CRLF for new lines.
        /// </param>
        /// <returns>If the request is successful, the OrderRef and AutoStartToken is returned.</returns>
        public static Task<SignResponse> SignAsync(this IBankIdApiClient apiClient, string endUserIp, string userVisibleData)
        {
            return apiClient.SignAsync(new SignRequest(endUserIp, userVisibleData));
        }

        /// <summary>
        /// Initiates an authentication order. Use the collect method to query the status of the order.
        /// </summary>
        /// <param name="apiClient">The <see cref="IBankIdApiClient"/> instance.</param>
        /// <param name="endUserIp">
        /// The user IP address as seen by RP.String.IPv4 and IPv6 is allowed.
        /// Note the importance of using the correct IP address.It must be the IP address representing the user agent (the end user device) as seen by the RP.
        /// If there is a proxy for inbound traffic, special considerations may need to be taken to get the correct address.
        ///
        /// In some use cases the IP address is not available, for instance for voice based services.
        /// In this case, the internal representation of those systems IP address is ok to use.
        /// </param>
        /// <param name="userVisibleData">
        /// The text to be displayed and signed. The text can be formatted using CR, LF and CRLF for new lines.
        /// </param>
        /// <param name="userNonVisibleData">
        /// Data not displayed to the user.
        /// </param>
        /// <returns>If the request is successful, the OrderRef and AutoStartToken is returned.</returns>
        public static Task<SignResponse> SignAsync(this IBankIdApiClient apiClient, string endUserIp, string userVisibleData, byte[] userNonVisibleData)
        {
            return apiClient.SignAsync(new SignRequest(endUserIp, userVisibleData, userNonVisibleData));
        }

        /// <summary>
        /// Initiates an authentication order. Use the collect method to query the status of the order.
        /// </summary>
        /// <param name="apiClient">The <see cref="IBankIdApiClient"/> instance.</param>
        /// <param name="endUserIp">
        /// The user IP address as seen by RP.String.IPv4 and IPv6 is allowed.
        /// Note the importance of using the correct IP address.It must be the IP address representing the user agent (the end user device) as seen by the RP.
        /// If there is a proxy for inbound traffic, special considerations may need to be taken to get the correct address.
        ///
        /// In some use cases the IP address is not available, for instance for voice based services.
        /// In this case, the internal representation of those systems IP address is ok to use.
        /// </param>
        /// <param name="userVisibleData">
        /// The text to be displayed and signed. The text can be formatted using CR, LF and CRLF for new lines.
        /// </param>
        /// <param name="userNonVisibleData">
        /// Data not displayed to the user.
        /// </param>
        /// <param name="personalIdentityNumber">
        /// The personal number of the user. 12 digits, century must be included (YYYYMMDDSSSC).
        /// If the personal number is excluded, the client must be started with the AutoStartToken returned in the response.
        /// </param>
        /// <returns>If the request is successful, the OrderRef and AutoStartToken is returned.</returns>
        public static Task<SignResponse> SignAsync(this IBankIdApiClient apiClient, string endUserIp, string userVisibleData, byte[] userNonVisibleData, string personalIdentityNumber)
        {
            return apiClient.SignAsync(new SignRequest(endUserIp, userVisibleData, userNonVisibleData, personalIdentityNumber));
        }

        /// <summary>
        /// Collects the result of a sign or auth order using the OrderRef as reference.
        /// RP should keep on calling collect every two seconds as long as status indicates pending.
        /// RP must abort if status indicates failed.
        /// </summary>
        /// <param name="apiClient">The <see cref="IBankIdApiClient"/> instance.</param>
        /// <param name="orderRef">The OrderRef returned from auth or sign.</param>
        /// <returns>The user identity is returned when complete.</returns>
        public static Task<CollectResponse> CollectAsync(this IBankIdApiClient apiClient, string orderRef)
        {
            return apiClient.CollectAsync(new CollectRequest(orderRef));
        }

        /// <summary>
        /// Cancels an ongoing sign or auth order.
        /// This is typically used if the user cancels the order in your service or app.
        /// </summary>
        /// <param name="apiClient">The <see cref="IBankIdApiClient"/> instance.</param>
        /// <param name="orderRef">The OrderRef returned from auth or sign.</param>
        public static Task<CancelResponse> CancelAsync(this IBankIdApiClient apiClient, string orderRef)
        {
            return apiClient.CancelAsync(new CancelRequest(orderRef));
        }
    }
}
