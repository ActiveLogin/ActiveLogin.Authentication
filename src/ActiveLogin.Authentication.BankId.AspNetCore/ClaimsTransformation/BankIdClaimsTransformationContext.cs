using System.Collections.Generic;
using System.Security.Claims;

namespace ActiveLogin.Authentication.BankId.AspNetCore.ClaimsTransformation
{
    public class BankIdClaimsTransformationContext
    {
        internal BankIdClaimsTransformationContext(BankIdOptions bankIdOptions, string bankIdOrderRef, string personalIdentityNumber, string name, string givenName, string surname)
        {
            BankIdOptions = bankIdOptions;

            BankIdOrderRef = bankIdOrderRef;

            PersonalIdentityNumber = personalIdentityNumber;

            Name = name;
            GivenName = givenName;
            Surname = surname;
        }

        public List<Claim> Claims { get; set; } = new List<Claim>();

        public BankIdOptions BankIdOptions { get; }

        public string BankIdOrderRef { get; }

        public string PersonalIdentityNumber { get; }

        public string Name { get; }
        public string GivenName { get; }
        public string Surname { get; }
    }
}
