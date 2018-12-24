using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    [DataContract]
    public class BankIdSessionStateUserAttributes
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
        private string notBefore { get; set; }
        public DateTime NotBefore => DateTime.Parse(notBefore).ToUniversalTime();

        [DataMember(Name = "notAfter")]
        private string notAfter { get; set; }
        public DateTime NotAfter => DateTime.Parse(notAfter).ToUniversalTime();

        [DataMember(Name = "ipAddress")]
        public string IpAddress { get; set; }
    }
}