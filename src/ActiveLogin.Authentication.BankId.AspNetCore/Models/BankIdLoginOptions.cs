using System.Collections.Generic;
using ActiveLogin.Identity.Swedish;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Models
{
    public class BankIdLoginOptions
    {
        public BankIdLoginOptions(
            List<string> certificatePolicies,
            SwedishPersonalIdentityNumber personalIdentityNumber,
            bool allowChangingPersonalIdentityNumber,
            bool autoLaunch,
            bool allowBiometric,
            bool useQrCode,
            bool allowForwardedHeaders)
        {
            CertificatePolicies = certificatePolicies;
            PersonalIdentityNumber = personalIdentityNumber;
            AllowChangingPersonalIdentityNumber = allowChangingPersonalIdentityNumber;
            AutoLaunch = autoLaunch;
            AllowBiometric = allowBiometric;
            UseQrCode = useQrCode;
            AllowForwardedHeaders = allowForwardedHeaders;
        }

        public List<string> CertificatePolicies { get; }

        public SwedishPersonalIdentityNumber PersonalIdentityNumber { get; }
        public bool AllowChangingPersonalIdentityNumber { get; }

        public bool AutoLaunch { get; }

        public bool AllowBiometric { get; }

        public bool UseQrCode { get; set; }

        public bool AllowForwardedHeaders { get; }
    }
}
