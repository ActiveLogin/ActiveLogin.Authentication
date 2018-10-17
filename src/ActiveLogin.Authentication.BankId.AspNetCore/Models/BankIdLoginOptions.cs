using ActiveLogin.Identity.Swedish;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Models
{
    public class BankIdLoginOptions
    {
        public BankIdLoginOptions(string certificatePolicies, SwedishPersonalIdentityNumber personalIdentityNumber, bool allowChangingPersonalIdentityNumber, bool autoLaunch)
        {
            CertificatePolicies = certificatePolicies;
            PersonalIdentityNumber = personalIdentityNumber;
            AllowChangingPersonalIdentityNumber = allowChangingPersonalIdentityNumber;
            AutoLaunch = autoLaunch;
        }

        public string CertificatePolicies { get; } = string.Empty;

        public SwedishPersonalIdentityNumber PersonalIdentityNumber { get; } = null;
        public bool AllowChangingPersonalIdentityNumber { get; } = true;

        public bool AutoLaunch { get; } = false;
    }
}