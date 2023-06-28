using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models;

/// <summary>
/// Information related to the verification of the authenticated digital ID cardholder.
/// </summary>
public class VerifyVerification
{
    public VerifyVerification(string verificationId, string verifiedAt, string signature)
    {
        VerificationId = verificationId;
        VerifiedAt = verifiedAt;
        Signature = signature;
    }

    /// <summary>
    /// Unique identifier for the performed verification, UUID.
    /// </summary>
    [JsonPropertyName("verificationId")]
    public string VerificationId { get; }

    /// <summary>
    /// Timestamp in ISO 8601 indicating date and time in UTC when the verification of the digital ID card was performed.
    /// </summary>
    [JsonPropertyName("verifiedAt")]
    public string VerifiedAt { get; }

    /// <summary>
    /// Base64-encoded enveloping XAdES signature conforming to ETSI TS 103 171 v2.1.1 Baseline Profile B-B.
    /// See section Signature for detailed information about the contents of the signature.
    /// </summary>
    [JsonPropertyName("signature")]
    public string Signature { get; }
}
