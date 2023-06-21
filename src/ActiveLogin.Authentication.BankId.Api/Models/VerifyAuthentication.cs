using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models;

/// <summary>
/// Information related to the BankID authentication of the digital ID cardholder during 'open ID card'.
/// </summary>
public class VerifyAuthentication
{
    public VerifyAuthentication(string identifiedAt, string orderRef, string signature, string ocspResponse)
    {
        IdentifiedAt = identifiedAt;
        OrderRef = orderRef;
        Signature = signature;
        OcspResponse = ocspResponse;
    }

    /// <summary>
    /// Timestamp in ISO 8601 indicating date and time in UTC when digital ID cardholder was identified using BankID.
    /// </summary>
    [JsonPropertyName("identifiedAt")]
    public string IdentifiedAt { get; }

    /// <summary>
    /// The orderRef received during BankID authentication in 'open ID card'
    /// </summary>
    [JsonPropertyName("orderRef")]
    public string OrderRef { get; }

    /// <summary>
    /// The signature received during BankID authentication in 'open ID card'. The content of the signature is described in BankID Signature Profile specification. String. Base64-encoded. XML signature.
    /// </summary>
    [JsonPropertyName("signature")]
    public string Signature { get; }

    /// <summary>
    /// The OCSP response received during BankID authentication in 'open ID card'.
    /// </summary>
    [JsonPropertyName("ocspResponse")]
    public string OcspResponse { get; }
}
