using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models;

/// <summary>
/// Parameters for the device where the BankID app is running.
/// </summary>
/// <param name="appIdentifier"></param>
/// <param name="deviceOs"></param>
/// <param name="deviceModelName"></param>
/// <param name="deviceIdentifier"></param>
public sealed class AppDeviceParameters(
    string appIdentifier,
    string deviceOs,
    string deviceModelName,
    string deviceIdentifier)
    : DeviceParameters(deviceIdentifier)
{
    /// <summary>
    /// The identifier of your application.
    /// This is the package name on Android and the bundle identifier on iOS.
    /// It is vital to use the correct value.
    /// If your service does not supply the correct value,
    /// legitimate orders might be blocked.
    /// </summary>
    [JsonPropertyName("appIdentifier")]
    public string? AppIdentifier { get; set; } = appIdentifier;

    /// <summary>
    /// The device operating system where your app is running.
    /// </summary>
    [JsonPropertyName("deviceOS")]
    public string? DeviceOs { get; set; } = deviceOs;

    /// <summary>
    /// The model of the device your app is running on.
    /// </summary>
    [JsonPropertyName("deviceModelName")]
    public string? DeviceModelName { get; set; } = deviceModelName;
}
