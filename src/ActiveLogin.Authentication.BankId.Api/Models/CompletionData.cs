using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models
{
    [DataContract]
    public class CompletionData
    {
        [DataMember(Name = "user")]
        public User User { get; set; }

        [DataMember(Name = "device")]
        public Device Device { get; set; }

        [DataMember(Name = "cert")]
        public Cert Cert { get; set; }

        [DataMember(Name = "signature")]
        public string Signature { get; set; }

        [DataMember(Name = "ocspResponse")]
        public string OcspResponse { get; set; }
    }
}