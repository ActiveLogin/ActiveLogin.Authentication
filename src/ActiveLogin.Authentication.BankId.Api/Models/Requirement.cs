using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models
{
    [DataContract]
    public class Requirement
    {
        [DataMember(Name = "certificatePolicies", EmitDefaultValue = false)]
        public string CertificatePolicies { get; set; }

        [DataMember(Name = "autoStartTokenRequired", EmitDefaultValue = false)]
        public bool? AutoStartTokenRequired { get; set; }

        [DataMember(Name = "allowFingerprint", EmitDefaultValue = false)]
        public bool? AllowFingerprint { get; set; }
    }
}