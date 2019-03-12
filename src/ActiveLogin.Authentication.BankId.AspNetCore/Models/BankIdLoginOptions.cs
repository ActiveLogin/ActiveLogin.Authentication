using System.Collections.Generic;
using ActiveLogin.Identity.Swedish;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Models
{
    public class BankIdLoginOptions
    {
        public BankIdLoginOptions(List<string> certificatePolicies,
            SwedishPersonalIdentityNumber personalIdentityNumber,
            bool allowChangingPersonalIdentityNumber,
            bool autoLaunch,
            bool allowBiometric)
        {
            CertificatePolicies = certificatePolicies;
            PersonalIdentityNumber = personalIdentityNumber;
            AllowChangingPersonalIdentityNumber = allowChangingPersonalIdentityNumber;
            AutoLaunch = autoLaunch;
            AllowBiometric = allowBiometric;
        }

        public List<string> CertificatePolicies { get; }

        public SwedishPersonalIdentityNumber PersonalIdentityNumber { get; }

        public bool AllowChangingPersonalIdentityNumber { get; }

        public bool AutoLaunch { get; }

        public bool AllowBiometric { get; }
    }
}
