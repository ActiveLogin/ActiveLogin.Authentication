using ActiveLogin.Authentication.BankId.Api.Models;
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
        bool requireRisk,
        PersonalIdentityNumber? requiredPersonalIdentityNumber = null,
        CardReader? cardReader = null
    )
    {
        CertificatePolicies = certificatePolicies;
        SameDevice = sameDevice;
        RequirePinCode = requirePinCode;
        RequireMrtd = requireMrtd;
        ReturnRisk = requireRisk;
        RequiredPersonalIdentityNumber = requiredPersonalIdentityNumber;
        CardReader = cardReader;
    }

    public List<BankIdCertificatePolicy> CertificatePolicies { get; } = new();

    public bool SameDevice { get; }

    public bool RequirePinCode { get; }

    public bool RequireMrtd { get; }

    public bool ReturnRisk {  get; }

    public PersonalIdentityNumber? RequiredPersonalIdentityNumber { get; }

    public CardReader? CardReader { get; }
}
