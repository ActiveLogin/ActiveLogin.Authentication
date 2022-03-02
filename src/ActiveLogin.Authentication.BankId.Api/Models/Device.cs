using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models
{
    /// <summary>
    /// Information related to the device.
    /// </summary>
    public class Device
    {
        public Device(string ipAddress)
        {
            IpAddress = ipAddress;
        }

        /// <summary>
        /// The IP address of the user agent as the BankID server discovers it.
        /// </summary>
        [JsonPropertyName("ipAddress")]
        public string IpAddress { get; private set; }
    }
}
