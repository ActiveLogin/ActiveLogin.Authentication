using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models;

/// <summary>
/// PhoneAuth request parameters.
/// </summary>
public class PhoneAuthRequest
{
    /// <summary></summary>
    /// <param name="personalNumber">
    /// The personal number of the user. String. 12 digits.
    /// </param>
    /// <param name="callInitiator">
    /// Indicate if the user or the RP initiated the phone call.
    /// user: user called the RP
    /// RP: RP called the user
    /// </param>
    /// <param name="requirement">Requirements on how the auth or sign order must be performed.</param>
    public PhoneAuthRequest(
        string personalNumber,
        CollectCallInitiator callInitiator,
        string? requirement = null)
    {
        PersonalNumber = personalNumber;
        CallInitiator = callInitiator;
    }

    /// <summary>
    /// The personal number of the user. String. 12 digits.
    /// </summary>
    [JsonPropertyName("personalNumber")]
    public string PersonalNumber { get; }

    /// <summary>
    /// Indicate if the user or the RP initiated the phone call.
    /// user: user called the RP
    /// RP: RP called the user
    /// </summary>
    [JsonPropertyName("callInitiator")]
    public CollectCallInitiator CallInitiator { get; }

    /// <summary>
    /// Indicate if the user or the RP initiated the phone call.
    /// user: user called the RP
    /// RP: RP called the user
    /// </summary>
    [JsonPropertyName("requirement")]
    public string? Requirement { get; }
}
