using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models
{
    [DataContract]
    public class Device
    {
        [DataMember(Name = "ipAddress")]
        public string IpAddress { get; set; }
    }
}