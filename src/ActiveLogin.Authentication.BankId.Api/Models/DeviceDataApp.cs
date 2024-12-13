using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models;

/// <summary>
/// Parameters for the device where the BankID app is running.
/// </summary>
/// <param name="appIdentifier">
/// The identifier of your application.
/// This is the package name on Android and the bundle identifier on iOS.
/// It is vital to use the correct value.If your service does not supply the correct value legitimate orders might be blocked.</param>
/// E.g. "com.example.myapp"
/// <param name="deviceOs">
/// The device operating system where your app is running.
/// E.g. "IOS 16.7.7"
/// </param>
/// <param name="deviceModelName">
/// The model of the device your app is running on.
/// E.g. "Apple iPhone14,3"
/// </param>
/// <param name="deviceIdentifier">
/// <inheritdoc cref="DeviceData(string)" path="/param[@name='deviceIdentifier']"/>
/// </param>
public sealed class DeviceDataApp(
    string appIdentifier,
    string deviceOs,
    string deviceModelName,
    string deviceIdentifier)
    : DeviceData(deviceIdentifier)
{
    /// <summary>
    /// Application Identifier, e.g. package name on Android and the bundle identifier on iOS.
    /// </summary>
    [JsonPropertyName("appIdentifier")]
    public string? AppIdentifier { get; set; } = appIdentifier;

    /// <summary>
    /// Device operating system.
    /// </summary>
    [JsonPropertyName("deviceOS")]
    public string? DeviceOs { get; set; } = deviceOs;

    /// <summary>
    /// Device model
    /// </summary>
    [JsonPropertyName("deviceModelName")]
    public string? DeviceModelName { get; set; } = deviceModelName;

}

