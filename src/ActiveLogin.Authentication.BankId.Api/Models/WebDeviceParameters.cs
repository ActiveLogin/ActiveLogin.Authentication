using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models;

/// <summary>
/// Parameters for the browser where the BankID app is running.
/// </summary>
/// <param name="referringDomain">
/// The domain that starts the BankID app.
/// This should generally be your domain name followed by the public suffix,
/// which will generally be the top level domain.
/// E.g. "example.com"
/// </param>
/// <param name="userAgent">
/// The user agent of the user interacting with your web page.
/// E.g. "Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:109.0) Gecko/20100101 Firefox/114.0"
/// </param>
/// <param name="deviceIdentifier">
/// The identifier of the device running your client.
/// Do not use a session cookie.Use a separate cookie or the hash of one.
/// This value should be unique to the user's browser and persist across sessions.
/// </param>
public sealed class WebDeviceParameters(
    string referringDomain,
    string userAgent,
    string deviceIdentifier)
    : DeviceParameters(deviceIdentifier)
{
    /// <summary>
    /// The domain that starts the BankID app.
    /// </summary>
    [JsonPropertyName("referringDomain")]
    public string? ReferringDomain { get; set; } = referringDomain;

    /// <summary>
    /// The user agent of the user interacting with your web page.
    /// </summary>
    [JsonPropertyName("userAgent")]
    public string? UserAgent { get; set; } = userAgent;

}
