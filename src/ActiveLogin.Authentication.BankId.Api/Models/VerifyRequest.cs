using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models;

/// <summary>
/// Verify request parameters.
/// </summary>
public class VerifyRequest
{
    /// <summary></summary>
    /// <param name="qrCode">The complete content of the scanned QR code.</param>
    public VerifyRequest(string qrCode)
    {
        QrCode = qrCode;
    }

    /// <summary>
    /// The complete content of the scanned QR code.
    /// </summary>
    [JsonPropertyName("qrCode")]
    public string QrCode { get; }
}
