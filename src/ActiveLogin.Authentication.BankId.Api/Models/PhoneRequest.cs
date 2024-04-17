using System.Text;
using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models;
public abstract class PhoneRequest
{
    /// <summary></summary>
    /// <param name="personalIdentityNumber">
    /// The personal number of the user. String. 12 digits.
    /// </param>
    /// <param name="callInitiator">
    /// Indicate if the user or the RP initiated the phone call. user: user called the RP. RP: RP called the user.
    /// </param>
    /// <param name="userVisibleData">
    /// The text to be displayed and signed. The text can be formatted using CR, LF and CRLF for new lines.
    /// </param>
    ///
    public PhoneRequest(string personalIdentityNumber, CallInitiator callInitiator, string userVisibleData)
        : this(personalIdentityNumber, callInitiator, userVisibleData, null, null, null)
    {
    }

    /// <summary></summary>
    /// <param name="personalIdentityNumber">
    /// The personal number of the user. String. 12 digits.
    /// </param>
    /// <param name="callInitiator">
    /// Indicate if the user or the RP initiated the phone call. user: user called the RP. RP: RP called the user.
    /// </param>
    /// <param name="userVisibleData">
    /// The text to be displayed and signed. The text can be formatted using CR, LF and CRLF for new lines.
    /// </param>
    /// <param name="userNonVisibleData">
    /// Data not displayed to the user.
    /// </param>
    public PhoneRequest(string personalIdentityNumber, CallInitiator callInitiator, string userVisibleData,
        byte[] userNonVisibleData)
        : this(personalIdentityNumber, callInitiator, userVisibleData, userNonVisibleData, null, null)
    {
    }

    /// <summary></summary>
    /// <param name="personalIdentityNumber">
    /// The personal number of the user. String. 12 digits.
    /// </param>
    /// <param name="callInitiator">
    /// Indicate if the user or the RP initiated the phone call. user: user called the RP. RP: RP called the user.
    /// </param>
    /// <param name="userVisibleData">
    /// The text to be displayed and signed. The text can be formatted using CR, LF and CRLF for new lines.
    /// </param>
    /// <param name="userVisibleDataFormat">
    /// If present, and set to "simpleMarkdownV1", this parameter indicates that userVisibleData holds formatting characters which, if used correctly, will make the text displayed with the user nicer to look at.
    /// For further information of formatting options, please study the document Guidelines for Formatted Text.
    /// </param>
    /// <param name="userNonVisibleData">
    /// Data not displayed to the user.
    /// </param>
    public PhoneRequest(string personalIdentityNumber, CallInitiator callInitiator, string userVisibleData,
        string userVisibleDataFormat, byte[] userNonVisibleData)
        : this(personalIdentityNumber, callInitiator, userVisibleData, userNonVisibleData, null, userVisibleDataFormat)
    {
    }

    /// <summary></summary>
    /// <param name="personalIdentityNumber">
    /// The personal number of the user. String. 12 digits.
    /// </param>
    /// <param name="callInitiator">
    /// Indicate if the user or the RP initiated the phone call. user: user called the RP. RP: RP called the user.
    /// </param>
    /// <param name="userVisibleData">
    /// The text to be displayed and signed. The text can be formatted using CR, LF and CRLF for new lines.
    /// </param>
    /// <param name="userNonVisibleData">
    /// Data not displayed to the user.
    /// </param>
    /// <param name="requirement">Requirements on how the phone auth or phone sign order must be performed.</param>
    public PhoneRequest(string personalIdentityNumber, CallInitiator callInitiator, string userVisibleData,
        byte[]? userNonVisibleData, PhoneRequirement? requirement)
        : this(personalIdentityNumber, callInitiator, userVisibleData, userNonVisibleData, requirement, null)
    {
    }

    /// <summary></summary>
    /// <param name="personalIdentityNumber">
    /// The personal number of the user. String. 12 digits.
    /// </param>
    /// <param name="callInitiator">
    /// Indicate if the user or the RP initiated the phone call. user: user called the RP. RP: RP called the user.
    /// </param>
    /// <param name="userVisibleData">
    /// The text to be displayed and signed. The text can be formatted using CR, LF and CRLF for new lines.
    /// </param>
    /// <param name="userNonVisibleData">
    /// Data not displayed to the user.
    /// </param>
    /// <param name="requirement">Requirements on how the phone auth or phone sign order must be performed.</param>
    /// <param name="userVisibleDataFormat">
    /// If present, and set to "simpleMarkdownV1", this parameter indicates that userVisibleData holds formatting characters which, if used correctly, will make the text displayed with the user nicer to look at.
    /// For further information of formatting options, please study the document Guidelines for Formatted Text.
    /// </param>
    public PhoneRequest(string personalIdentityNumber, CallInitiator callInitiator, string? userVisibleData,
        byte[]? userNonVisibleData, PhoneRequirement? requirement, string? userVisibleDataFormat)
    {
        if (this is PhoneSignRequest && userVisibleData == null)
        {
            throw new ArgumentNullException(nameof(userVisibleData));
        }

        PersonalIdentityNumber = personalIdentityNumber ?? throw new ArgumentNullException(nameof(personalIdentityNumber));
        CallInitiator = ParseCallInitiator(callInitiator);
        UserVisibleData = ToBase64EncodedString(userVisibleData);
        UserNonVisibleData = ToBase64EncodedString(userNonVisibleData);
        Requirement = requirement ?? new PhoneRequirement();
        UserVisibleDataFormat = userVisibleDataFormat;
    }

    /// <summary>
    /// The personal number of the user. String. 12 digits.
    /// </summary>
    [JsonPropertyName("personalNumber")]
    public string PersonalIdentityNumber { get; }

    /// <summary>
    /// Indicate if the user or the RP initiated the phone call. user: user called the RP. RP: RP called the user.
    /// </summary>
    [JsonPropertyName("callInitiator")]
    public string CallInitiator { get; }

    /// <summary>
    /// Requirements on how the phone auth or phone sign order must be performed.
    /// </summary>
    [JsonPropertyName("requirement"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public PhoneRequirement Requirement { get; }

    /// <summary>
    /// The text can be formatted using CR, LF and CRLF for new lines.
    /// The text must be encoded as UTF-8 and then base 64 encoded.
    /// 1—1 500 characters after base 64encoding.
    ///
    /// Scenario sign: The text to be displayed and signed. String. The text can be formatted using CR, LF and CRLF for new lines.
    ///
    /// Scenario auth: A text that is displayed to the user during authentication with BankID, with the
    /// purpose of providing context for the authentication and to enable users to notice if
    /// there is something wrong about the identification and avoid attempted frauds.
    /// </summary>
    [JsonPropertyName("userVisibleData"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? UserVisibleData { get; }

    /// <summary>
    /// If present, and set to "simpleMarkdownV1", this parameter indicates that userVisibleData holds formatting characters which, if used correctly, will make the text displayed with the user nicer to look at.
    /// For further information of formatting options, please study the document Guidelines for Formatted Text.
    /// </summary>
    [JsonPropertyName("userVisibleDataFormat"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? UserVisibleDataFormat { get; }

    /// <summary>
    /// Data not displayed to the user.
    /// </summary>
    [JsonPropertyName("userNonVisibleData"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? UserNonVisibleData { get; }

    private static string ParseCallInitiator(CallInitiator callInitiator)
    {
        return callInitiator switch
        {
            Models.CallInitiator.User => callInitiator.ToString().ToLower(),
            Models.CallInitiator.RP => callInitiator.ToString().ToUpper(),
            _ => throw new ArgumentException(
                $"Call initiator must be {Models.CallInitiator.User} or {Models.CallInitiator.RP}",
                nameof(callInitiator))
        };
    }

    private static string? ToBase64EncodedString(string? value)
    {
        return value == null ? null : Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
    }

    private static string? ToBase64EncodedString(byte[]? value)
    {
        return value == null ? null : Convert.ToBase64String(value);
    }
}
