using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models;

public abstract class DeviceParameters(string deviceIdentifier)
{
    /// <summary>
    /// The identifier of the device your client is running on.
    /// This is used to uniquely identify the device and should be a
    /// value that is not tied to a single user of the device.
    /// Preferably, it should remain the same even if our app is reinstalled.
    /// </summary>
    [JsonPropertyName("deviceIdentifier")]
    public string? DeviceIdentifier { get; } = deviceIdentifier;
}
