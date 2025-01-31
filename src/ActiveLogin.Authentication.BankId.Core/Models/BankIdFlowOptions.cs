using ActiveLogin.Authentication.BankId.Core.CertificatePolicies;
using ActiveLogin.Authentication.BankId.Core.Risk;
using ActiveLogin.Identity.Swedish;

namespace ActiveLogin.Authentication.BankId.Core.Models;

public class BankIdFlowOptions
{
    public BankIdFlowOptions(
        List<BankIdCertificatePolicy> certificatePolicies,
        BankIdAllowedRiskLevel allowedRiskLevel,
        bool sameDevice,
        bool requirePinCode,
        bool requireMrtd,
        bool requireRisk,
        PersonalIdentityNumber? requiredPersonalIdentityNumber = null)
    {
        CertificatePolicies = certificatePolicies;
        AllowedRiskLevel = allowedRiskLevel;
        SameDevice = sameDevice;
        RequirePinCode = requirePinCode;
        RequireMrtd = requireMrtd;
        ReturnRisk = requireRisk;
        RequiredPersonalIdentityNumber = requiredPersonalIdentityNumber;
    }

    public List<BankIdCertificatePolicy> CertificatePolicies { get; }

    public bool SameDevice { get; }

    public bool RequirePinCode { get; }

    public bool RequireMrtd { get; }

    public bool ReturnRisk {  get; }

    public BankIdAllowedRiskLevel AllowedRiskLevel { get; }

    public PersonalIdentityNumber? RequiredPersonalIdentityNumber { get; }

}
