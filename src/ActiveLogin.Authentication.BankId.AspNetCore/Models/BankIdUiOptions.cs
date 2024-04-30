using ActiveLogin.Authentication.BankId.Core.CertificatePolicies;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Models;

public class BankIdUiOptions
{
    public BankIdUiOptions(
        List<BankIdCertificatePolicy> certificatePolicies,
        bool sameDevice,
        bool requirePinCode,
        bool requireMrtd,
        string cancelReturnUrl,
        string stateCookieName)
    {
        CertificatePolicies = certificatePolicies;
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

    public string CancelReturnUrl { get; }

    public string StateCookieName { get; }
}
