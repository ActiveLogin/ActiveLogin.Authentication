using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    /// <summary>
    /// Information related to the device.
    /// </summary>
    [DataContract]
    public class Device
    {
        /// <summary>
        /// The IP address of the user agent as the BankID server discovers it.
        /// </summary>
        [DataMember(Name = "ipAddress")]
        public string IpAddress { get; set; }
    }
}