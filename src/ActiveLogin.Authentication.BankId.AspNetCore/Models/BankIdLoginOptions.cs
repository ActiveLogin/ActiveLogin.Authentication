using System.Collections.Generic;
using ActiveLogin.Identity.Swedish;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Models
{
    public class BankIdLoginOptions
    {
        public BankIdLoginOptions(
            List<string> certificatePolicies,
            SwedishPersonalIdentityNumber? personalIdentityNumber,
            bool allowChangingPersonalIdentityNumber,
            bool sameDevice,
            bool allowBiometric,
            bool useQrCode,
            string cancelReturnUrl,
            string stateCookieName)
        {
            CertificatePolicies = certificatePolicies;
            PersonalIdentityNumber = personalIdentityNumber;
            AllowChangingPersonalIdentityNumber = allowChangingPersonalIdentityNumber;
            SameDevice = sameDevice;
            AllowBiometric = allowBiometric;
            UseQrCode = useQrCode;
            CancelReturnUrl = cancelReturnUrl;
            StateCookieName = stateCookieName;
        }

        public List<string> CertificatePolicies { get; }

        public SwedishPersonalIdentityNumber? PersonalIdentityNumber { get; }
        public bool AllowChangingPersonalIdentityNumber { get; }

        public bool SameDevice { get; }

        public bool AllowBiometric { get; }

        public bool UseQrCode { get; set; }

        public string CancelReturnUrl { get; }

        public string StateCookieName { get; }
    }
}
