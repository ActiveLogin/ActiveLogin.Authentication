using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    [DataContract]
    public class BankIdGetSessionUserAttributes
    {
        internal BankIdGetSessionUserAttributes(string signature, string givenName, string surname, string name, string personalIdentityNumber, string notBefore, string notAfter, string ipAddress)
        {
            Signature = signature;
            GivenName = givenName;
            Surname = surname;
            Name = name;
            PersonalIdentityNumber = personalIdentityNumber;
            NotBefore = notBefore;
            NotAfter = notAfter;
            IpAddress = ipAddress;
        }

        [DataMember(Name = "SPAR")]
        public BankIdGetSessionUserAddressAttributes AddressAttributes { get; set; }

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
        public string NotBefore { get; private set; }

        [DataMember(Name = "notAfter")]
        public string NotAfter { get; private set; }

        [DataMember(Name = "ipAddress")]
        public string IpAddress { get; private set; }

        [DataContract]
        public class BankIdGetSessionUserAddressAttributes
        {
            [DataMember(Name = "spakoDatumFrom")]
            public string From { get; set; }

            [DataMember(Name = "spakoDatumTill")]
            public string To { get; set; }

            [DataMember(Name = "spakoUtdelningsadress2")]
            public string Address { get; set; }

            [DataMember(Name = "spakoPostNr")]
            public string ZipCode { get; set; }

            [DataMember(Name = "spakoPostort")]
            public string City { get; set; }

            [DataMember(Name = "spakoFolkbokfordLanKod")]
            public string County { get; set; }

            [DataMember(Name = "spakoFolkbokfordKommunKod")]
            public string Municipality { get; set; }

            [DataMember(Name = "spakoFolkbokforingsdatum")]
            public string Registered { get; set; }

            [DataMember(Name = "spakoDistriktKod")]
            public string District { get; set; }
        }
    }
}
