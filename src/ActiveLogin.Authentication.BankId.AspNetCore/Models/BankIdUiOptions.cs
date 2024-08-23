using ActiveLogin.Authentication.BankId.Core.CertificatePolicies;
using ActiveLogin.Authentication.BankId.Core.Risk;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Models;

public class BankIdUiOptions
{
    public BankIdUiOptions(
        List<BankIdCertificatePolicy> certificatePolicies,
        BankIdAllowedRiskLevel allowedRiskLevel,
        bool sameDevice,
        bool requirePinCode,
        bool requireMrtd,
        string cancelReturnUrl,
        string stateCookieName)
    {
        CertificatePolicies = certificatePolicies;
        AllowedRiskLevel = allowedRiskLevel;
        SameDevice = sameDevice;
        RequirePinCode = requirePinCode;
        RequireMrtd = requireMrtd;
        CancelReturnUrl = cancelReturnUrl;
        StateCookieName = stateCookieName;
    }

    public List<BankIdCertificatePolicy> CertificatePolicies { get; }

    public bool SameDevice { get; }

    public bool RequirePinCode { get; }

    public bool RequireMrtd { get; }

    public BankIdAllowedRiskLevel AllowedRiskLevel { get; }

    public string CancelReturnUrl { get; }

    public string StateCookieName { get; }
}
