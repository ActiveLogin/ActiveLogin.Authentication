using ActiveLogin.Authentication.BankId.Core.CertificatePolicies;
using ActiveLogin.Authentication.BankId.Core.Risk;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Models;

public class BankIdUiOptions(
    List<BankIdCertificatePolicy> certificatePolicies,
    BankIdAllowedRiskLevel allowedRiskLevel,
    bool sameDevice,
    bool requirePinCode,
    bool requireMrtd,
    bool returnRisk,
    string cancelReturnUrl,
    string stateCookieName
)
{

    public List<BankIdCertificatePolicy> CertificatePolicies { get; } = certificatePolicies;
    public BankIdAllowedRiskLevel AllowedRiskLevel { get; } = allowedRiskLevel;
    public bool SameDevice { get; } = sameDevice;
    public bool RequirePinCode { get; } = requirePinCode;
    public bool RequireMrtd { get; } = requireMrtd;
    public bool ReturnRisk { get; } = returnRisk;
    public string CancelReturnUrl { get; } = cancelReturnUrl;
    public string StateKeyCookieName { get; } = stateCookieName;
}
