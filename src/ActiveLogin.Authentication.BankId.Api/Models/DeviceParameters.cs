using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models;

public abstract class DeviceParameters(string deviceIdentifier)
{
    /// <summary>
    /// Unique identifier for the device/browser that BankId is running on.
    /// </summary>
    [JsonPropertyName("deviceIdentifier")]
    public string? DeviceIdentifier { get; } = deviceIdentifier;
}
