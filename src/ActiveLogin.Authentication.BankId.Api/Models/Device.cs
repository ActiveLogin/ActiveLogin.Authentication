using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models;

/// <summary>
/// Information related to the device.
/// </summary>
public class Device
{
    public Device(string ipAddress, string uhi)
    {
        IpAddress = ipAddress;
        Uhi = uhi;
    }

    /// <summary>
    /// The IP address of the user agent as the BankID server discovers it.
    /// </summary>
    [JsonPropertyName("ipAddress")]
    public string IpAddress { get; }

    /// <summary>
    /// A unique hardware id of the users device.
    /// </summary>
    [JsonPropertyName("uhi")]
    public string Uhi { get; }
}
