using ActiveLogin.Authentication.BankId.Core.CertificatePolicies;
using ActiveLogin.Identity.Swedish;

namespace ActiveLogin.Authentication.BankId.Core.Models;

public class BankIdFlowOptions
{
    public BankIdFlowOptions(
        List<BankIdCertificatePolicy> certificatePolicies,
        bool sameDevice,
        bool requirePinCode,
        bool requireMrtd,
        PersonalIdentityNumber? requiredPersonalIdentityNumber = null)
    {
        CertificatePolicies = certificatePolicies;
        SameDevice = sameDevice;
        RequirePinCode = requirePinCode;
        RequireMrtd = requireMrtd;
        RequiredPersonalIdentityNumber = requiredPersonalIdentityNumber;
    }

    public List<BankIdCertificatePolicy> CertificatePolicies { get; }

    public bool SameDevice { get; }

    public bool RequirePinCode { get; }

    public bool RequireMrtd { get; }

    public PersonalIdentityNumber? RequiredPersonalIdentityNumber { get; }

}
