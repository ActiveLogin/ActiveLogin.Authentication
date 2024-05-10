using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models;

public class CompletionData
{
    public CompletionData(User user, Device device, string bankIdIssueDate, StepUp? stepUp, string signature, string ocspResponse, string? risk)
    {
        User = user;
        Device = device;
        BankIdIssueDate = bankIdIssueDate;
        StepUp = stepUp;
        Signature = signature;
        OcspResponse = ocspResponse;
        Risk = risk;
    }

    /// <summary>
    /// Information related to the user.
    /// </summary>
    [JsonPropertyName("user")]
    public User User { get; }

    /// <summary>
    /// Information related to the device.
    /// </summary>
    [JsonPropertyName("device")]
    public Device Device { get; }

    /// <summary>
    /// The date the BankID was issued to the user. The issue date of the ID expressed using ISO 8601 date format YYYY-MM-DD with a UTC time zone offset.
    /// </summary>
    [JsonPropertyName("bankIdIssueDate")]
    public string BankIdIssueDate { get; }

    /// <summary>
    /// Information about extra verifications that were part of the transaction. 
    /// </summary>
    [JsonPropertyName("stepUp")]
    public StepUp? StepUp { get; }

    /// <summary>
    /// The signature. Base64-encoded.
    /// The content of the signature is described in BankID Signature Profile specification.
    /// </summary>
    [JsonPropertyName("signature")]
    public string Signature { get; }

    /// <summary>
    /// The OCSP response. String. Base64-encoded.
    /// The OCSP response is signed by a certificate that has the same issuer as the certificate being verified.
    /// The OSCP response has an extension for Nonce.
    /// </summary>
    [JsonPropertyName("ocspResponse")]
    public string OcspResponse { get; }

    /// <summary>
    /// Indicates the risk level of the order based on data available in the order. Only returned if requested in the order.
    /// </summary>
    [JsonPropertyName("risk")]
    public string? Risk { get; }
}
