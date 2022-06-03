namespace ActiveLogin.Authentication.BankId.Api.Models;

/// <summary>
/// Sign request parameters.
/// </summary>
public class SignRequest : Request
{
    /// <summary></summary>
    /// <param name="endUserIp">
    /// The user IP address as seen by RP. IPv4 and IPv6 is allowed.
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
    /// <param name="requirement">Requirements on how the auth or sign order must be performed.</param>
    /// <param name="userVisibleDataFormat">
    /// If present, and set to "simpleMarkdownV1", this parameter indicates that userVisibleData holds formatting characters which, if used correctly, will make the text displayed with the user nicer to look at.
    /// For further information of formatting options, please study the document Guidelines for Formatted Text.
    /// </param>
    public SignRequest(
        string endUserIp,
        string userVisibleData,
        byte[]? userNonVisibleData = null,
        string? personalIdentityNumber = null,
        Requirement? requirement = null,
        string? userVisibleDataFormat = null)
        : base(endUserIp, userVisibleData, userNonVisibleData, personalIdentityNumber, requirement, userVisibleDataFormat)
    {
    }
}
