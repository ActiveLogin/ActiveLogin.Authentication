using System.Text.Json;
using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models;

/// <summary>
/// Base class for device parameters.
/// </summary>
/// <param name="deviceIdentifier">
/// The identifier of the device your client is running on.
/// This is used to uniquely identify the device and should be a value
/// that is not tied to a single user of the device.
/// Preferably, it should remain the same even if your app is reinstalled.
/// </param>
public abstract class DeviceData(string deviceIdentifier) : IBankIdEndUserDeviceData
{
    /// The identifier of the device your client is running on.
    /// This is used to uniquely identify the device and should be a value
    /// that is not tied to a single user of the device.
    /// Preferably, it should remain the same even if your app is reinstalled.
    [JsonPropertyName("deviceIdentifier")]
    public string? DeviceIdentifier { get; } = deviceIdentifier;

}

