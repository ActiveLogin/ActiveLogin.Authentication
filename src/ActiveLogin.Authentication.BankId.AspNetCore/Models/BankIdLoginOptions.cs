using System.Runtime.Serialization;
using ActiveLogin.Identity.Swedish;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Models
{
    public class BankIdLoginOptions
    {
        public BankIdLoginOptions(string certificatePolicies, SwedishPersonalIdentityNumber personalIdentityNumber, bool allowChangingPersonalIdentityNumber)
        {
            CertificatePolicies = certificatePolicies;
            PersonalIdentityNumber = personalIdentityNumber;
            AllowChangingPersonalIdentityNumber = allowChangingPersonalIdentityNumber;
        }

        public string CertificatePolicies { get; } = string.Empty;

        public SwedishPersonalIdentityNumber PersonalIdentityNumber { get; } = null;
        public bool AllowChangingPersonalIdentityNumber { get; } = true;
    }
}