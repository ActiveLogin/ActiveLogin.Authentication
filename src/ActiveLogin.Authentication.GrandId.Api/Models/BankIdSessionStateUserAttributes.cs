using System;
using System.Runtime.Serialization;
using System.Text;

namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    [DataContract]
    public class BankIdSessionStateUserAttributes
    {
        internal BankIdSessionStateUserAttributes(string signatureRaw, string givenName, string surname, string name, string personalIdentityNumber, string notBeforeRaw, string notAfterRaw, string ipAddress)
        {
            SignatureRaw = signatureRaw;
            GivenName = givenName;
            Surname = surname;
            Name = name;
            PersonalIdentityNumber = personalIdentityNumber;
            NotBeforeRaw = notBeforeRaw;
            NotAfterRaw = notAfterRaw;
            IpAddress = ipAddress;
        }

        [DataMember(Name = "signature")]
        public string SignatureRaw { get; private set; }
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
        public string NotBeforeRaw { get; private set; }
        public DateTime NotBefore => DateTime.Parse(NotBeforeRaw).ToUniversalTime();

        [DataMember(Name = "notAfter")]
        public string NotAfterRaw { get; private set; }
        public DateTime NotAfter => DateTime.Parse(NotAfterRaw).ToUniversalTime();

        [DataMember(Name = "ipAddress")]
        public string IpAddress { get; private set; }
    }
}