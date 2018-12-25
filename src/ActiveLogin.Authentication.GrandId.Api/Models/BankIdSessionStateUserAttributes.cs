using System;
using System.Runtime.Serialization;
using System.Text;

namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    [DataContract]
    public class BankIdSessionStateUserAttributes
    {
        public BankIdSessionStateUserAttributes(string signatureRaw, string givenName, string surname, string name, string personalIdentityNumber, string notBefore, string notAfter, string ipAddress)
        {
            SignatureRaw = signatureRaw;
            GivenName = givenName;
            Surname = surname;
            Name = name;
            PersonalIdentityNumber = personalIdentityNumber;
            this.notBefore = notBefore;
            this.notAfter = notAfter;
            IpAddress = ipAddress;
        }

        [DataMember(Name = "signature")]
        public string SignatureRaw { get; set; }
        public string SignatureXml => Encoding.UTF8.GetString(Convert.FromBase64String(SignatureRaw));

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