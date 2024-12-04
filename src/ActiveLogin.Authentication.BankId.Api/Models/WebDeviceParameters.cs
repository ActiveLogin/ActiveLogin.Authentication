using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models;

/// <summary>
/// Parameters for the browser where the BankID app is running.
/// </summary>
/// <param name="referringDomain"></param>
/// <param name="userAgent"></param>
/// <param name="deviceIdentifier"></param>
public sealed class WebDeviceParameters(
    string referringDomain,
    string userAgent,
    string deviceIdentifier)
    : DeviceParameters(deviceIdentifier)
{
    /// <summary>
    /// The domain that starts the BankID app.
    /// This should generally be your domain name followed by the public suffix,
    /// which will generally be the top level domain.
    /// </summary>
    [JsonPropertyName("referringDomain")]
    public string? ReferringDomain { get; set; } = referringDomain;

    /// <summary>
    /// The user agent of the user interacting with your web page.
    /// </summary>
    [JsonPropertyName("userAgent")]
    public string? UserAgent { get; set; } = userAgent;
}
