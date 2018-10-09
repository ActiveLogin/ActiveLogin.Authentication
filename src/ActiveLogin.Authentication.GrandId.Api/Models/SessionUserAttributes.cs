using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    [DataContract]
    public class SessionUserAttributes
    {
        [DataMember(Name = "signature")]
        public string Signature { get; set; }

        [DataMember(Name = "givenName")]
        public string GivenName { get; set; }

        [DataMember(Name = "surname")]
        public string Surname { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "personalNumber")]
        public string PersonalIdentityNumber { get; set; }

        [DataMember(Name = "notBefore")]
        public string NotBefore { get; set; }

        [DataMember(Name = "notAfter")]
        public string NotAfter { get; set; }

        [DataMember(Name = "ipAddress")]
        public string IpAddress { get; set; }
    }
}