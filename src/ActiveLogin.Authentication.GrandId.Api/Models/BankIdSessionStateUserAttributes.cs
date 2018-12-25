using System;
using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    [DataContract]
    public class BankIdSessionStateUserAttributes
    {
        public BankIdSessionStateUserAttributes(string signature, string givenName, string surname, string name, string personalIdentityNumber, string notBefore, string notAfter, string ipAddress)
        {
            Signature = signature;
            GivenName = givenName;
            Surname = surname;
            Name = name;
            PersonalIdentityNumber = personalIdentityNumber;
            this.notBefore = notBefore;
            this.notAfter = notAfter;
            IpAddress = ipAddress;
        }

        [DataMember(Name = "signature")]
        public string Signature { get; private set; }

        [DataMember(Name = "givenName")]
        public string GivenName { get; private set; }

        [DataMember(Name = "surname")]
        public string Surname { get; private set; }

        [DataMember(Name = "name")]
        public string Name { get; private set; }

        [DataMember(Name = "personalNumber")]
        public string PersonalIdentityNumber { get; private set; }

        [DataMember(Name = "notBefore")]
        private string notBefore { get; set; }
        public DateTime NotBefore => DateTime.Parse(notBefore).ToUniversalTime();

        [DataMember(Name = "notAfter")]
        private string notAfter { get; set; }
        public DateTime NotAfter => DateTime.Parse(notAfter).ToUniversalTime();

        [DataMember(Name = "ipAddress")]
        public string IpAddress { get; private set; }
    }
}