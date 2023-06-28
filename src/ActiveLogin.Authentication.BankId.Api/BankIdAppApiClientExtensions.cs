using ActiveLogin.Authentication.BankId.Api.Models;

namespace ActiveLogin.Authentication.BankId.Api;

/// <summary>
/// Extensions to enable easier access to common api scenarios.
/// </summary>
public static class BankIdAppApiClientExtensions
{
    /// <summary></summary>
    /// <param name="appApiClient">The <see cref="IBankIdAppApiClient"/> instance.</param>
    /// <param name="endUserIp">
    /// The user IP address as seen by RP. IPv4 and IPv6 is allowed.
    /// Note the importance of using the correct IP address.It must be the IP address representing the user agent (the end user device) as seen by the RP.
    /// If there is a proxy for inbound traffic, special considerations may need to be taken to get the correct address.
    ///
    /// In some use cases the IP address is not available, for instance for voice based services.
    /// In this case, the internal representation of those systems IP address is ok to use.
    /// </param>
    /// <param name="requirement">Requirements on how the auth or sign order must be performed.</param>
    /// <param name="userVisibleData">
    /// The text to be displayed and signed. The text can be formatted using CR, LF and CRLF for new lines.
    /// </param>
    /// <param name="userNonVisibleData">
    /// Data not displayed to the user.
    /// </param>
    /// <param name="userVisibleDataFormat">
    /// If present, and set to "simpleMarkdownV1", this parameter indicates that userVisibleData holds formatting characters which, if used correctly, will make the text displayed with the user nicer to look at.
    /// For further information of formatting options, please study the document Guidelines for Formatted Text.
    /// </param>
    public static Task<AuthResponse> AuthAsync(
        this IBankIdAppApiClient appApiClient,
        string endUserIp,
        Requirement? requirement = null,
        string? userVisibleData = null,
        byte[]? userNonVisibleData = null,
        string? userVisibleDataFormat = null)
    {
        return appApiClient.AuthAsync(new(
            endUserIp,
            userVisibleData: userVisibleData,
            userNonVisibleData: userNonVisibleData,
            requirement: requirement,
            userVisibleDataFormat: userVisibleDataFormat));
    }

    /// <summary>
    /// Initiates an authentication order. Use the collect method to query the status of the order.
    /// </summary>
    /// <param name="appApiClient">The <see cref="IBankIdAppApiClient"/> instance.</param>
    /// <param name="endUserIp">
    /// The user IP address as seen by RP.String.IPv4 and IPv6 is allowed.
    /// Note the importance of using the correct IP address.It must be the IP address representing the user agent (the end user device) as seen by the RP.
    /// If there is a proxy for inbound traffic, special considerations may need to be taken to get the correct address.
    ///
    /// In some use cases the IP address is not available, for instance for voice based services.
    /// In this case, the internal representation of those systems IP address is ok to use.
    /// </param>
    /// <returns>If the request is successful, the OrderRef and AutoStartToken is returned.</returns>
    public static Task<AuthResponse> AuthAsync(this IBankIdAppApiClient appApiClient, string endUserIp)
    {
        return appApiClient.AuthAsync(new AuthRequest(endUserIp));
    }

    /// <summary>
    /// Initiates an authentication order. Use the collect method to query the status of the order.
    /// </summary>
    /// <param name="appApiClient">The <see cref="IBankIdAppApiClient"/> instance.</param>
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
    public static Task<SignResponse> SignAsync(this IBankIdAppApiClient appApiClient, string endUserIp, string userVisibleData)
    {
        return appApiClient.SignAsync(new SignRequest(endUserIp, userVisibleData));
    }

    /// <summary>
    /// Initiates an authentication order. Use the collect method to query the status of the order.
    /// </summary>
    /// <param name="appApiClient">The <see cref="IBankIdAppApiClient"/> instance.</param>
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
    public static Task<SignResponse> SignAsync(this IBankIdAppApiClient appApiClient, string endUserIp, string userVisibleData, byte[] userNonVisibleData)
    {
        return appApiClient.SignAsync(new SignRequest(
            endUserIp,
            userVisibleData,
            userNonVisibleData: userNonVisibleData));
    }

    /// <summary>
    /// Collects the result of a sign or auth order using the OrderRef as reference.
    /// RP should keep on calling collect every two seconds as long as status indicates pending.
    /// RP must abort if status indicates failed.
    /// </summary>
    /// <param name="appApiClient">The <see cref="IBankIdAppApiClient"/> instance.</param>
    /// <param name="orderRef">The OrderRef returned from auth or sign.</param>
    /// <returns>The user identity is returned when complete.</returns>
    public static Task<CollectResponse> CollectAsync(this IBankIdAppApiClient appApiClient, string orderRef)
    {
        return appApiClient.CollectAsync(new CollectRequest(orderRef));
    }

    /// <summary>
    /// Cancels an ongoing sign or auth order.
    /// This is typically used if the user cancels the order in your service or app.
    /// </summary>
    /// <param name="appApiClient">The <see cref="IBankIdAppApiClient"/> instance.</param>
    /// <param name="orderRef">The OrderRef returned from auth or sign.</param>
    public static Task<CancelResponse> CancelAsync(this IBankIdAppApiClient appApiClient, string orderRef)
    {
        return appApiClient.CancelAsync(new CancelRequest(orderRef));
    }
}
