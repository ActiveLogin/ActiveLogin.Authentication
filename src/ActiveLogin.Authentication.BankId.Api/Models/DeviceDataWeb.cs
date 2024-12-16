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
/// <inheritdoc cref="DeviceData(string)" path="/param[@name='deviceIdentifier']"/>
/// </param>
public sealed class DeviceDataWeb(
    string referringDomain,
    string userAgent,
    string deviceIdentifier)
    : DeviceData(deviceIdentifier)
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
