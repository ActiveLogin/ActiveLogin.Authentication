using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models;

/// <summary>
/// Collect response result.
/// </summary>
public class CollectResponse
{
    public CollectResponse(string orderRef, string status, string hintCode)
        : this(orderRef, status, hintCode, null, null)
    {
    }

    [JsonConstructor]
    public CollectResponse(string orderRef, string status, string hintCode, CompletionData? completionData, string? callInitiator)
    {
        OrderRef = orderRef;
        Status = status;
        CallInitiator = callInitiator;
        HintCode = hintCode;
        CompletionData = completionData;
    }

    /// <summary>
    /// The orderRef in question.
    /// </summary>
    [JsonPropertyName("orderRef")]
    public string OrderRef { get; }

    /// <summary>
    /// Collect status.
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; }

    /// <summary>
    /// Collect hint code.
    /// RP should use the HintCode to provide the user with details and instructions and keep on calling collect until failed or complete.
    /// </summary>
    /// <remarks>Only present for pending and failed orders.</remarks>
    [JsonPropertyName("hintCode")]
    public string HintCode { get; }

    /// <summary>
    /// Collect call initiator.
    /// Indicates whether RP or user initiated a phone/auth, if phone authentication.
    /// </summary>
    /// <remarks>Only present for phone authentication.</remarks>
    [JsonPropertyName("callInitiator")]
    public string? CallInitiator { get; }

    /// <summary>
    /// The completionData includes the signature, user information and the OCSP response.
    /// RP should control the user information and continue their process.
    /// RP should keep the completion data for future references/compliance/audit.
    /// </summary>
    /// <remarks>Only present for complete orders.</remarks>
    [JsonPropertyName("completionData")]
    public CompletionData? CompletionData { get; }
}
