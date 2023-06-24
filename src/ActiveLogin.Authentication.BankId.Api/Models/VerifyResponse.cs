using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models;

/// <summary>
/// Verify response result.
/// </summary>
public class VerifyResponse
{
    public VerifyResponse(string transactionType, VerifyUser user, VerifyVerification verification, VerifyAuthentication authentication)
    {
        TransactionType = transactionType;
        User = user;
        Verification = verification;
        Authentication = authentication;
    }

    /// <summary>
    /// Type of transaction, a fixed value of ID-kort-validering.
    /// </summary>
    [JsonPropertyName("transactionType")]
    public string TransactionType { get; }

    /// <summary>
    /// Information related to the authenticated ID cardholder.
    /// </summary>
    [JsonPropertyName("user")]
    public VerifyUser User { get; }

    /// <summary>
    /// Information related to the verification of the authenticated digital ID cardholder.
    /// </summary>
    [JsonPropertyName("verification")]
    public VerifyVerification Verification { get; }

    /// <summary>
    /// Information related to the authenticated ID cardholder.
    /// </summary>
    [JsonPropertyName("authentication")]
    public VerifyAuthentication Authentication { get; }
}
