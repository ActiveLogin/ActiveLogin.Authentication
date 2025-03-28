using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.Core.CertificatePolicies;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Models;

public class BankIdUiOptions
{
    public BankIdUiOptions(
        List<BankIdCertificatePolicy> certificatePolicies,
        bool sameDevice,
        bool requirePinCode,
        bool requireMrtd,
        bool returnRisk,
        string cancelReturnUrl,
        string stateCookieName,
        CardReader? cardReader
    )
    {
        CertificatePolicies = certificatePolicies;
        SameDevice = sameDevice;
        RequirePinCode = requirePinCode;
        RequireMrtd = requireMrtd;
        ReturnRisk = returnRisk;
        CancelReturnUrl = cancelReturnUrl;
        StateKeyCookieName = stateCookieName;
        CardReader = cardReader;
    }

    public List<BankIdCertificatePolicy> CertificatePolicies { get; }

    public bool SameDevice { get; }

    public bool RequirePinCode { get; }

    public bool RequireMrtd { get; }

    public bool ReturnRisk { get; }

    public string CancelReturnUrl { get; }

    public string StateKeyCookieName { get; }

    public CardReader? CardReader { get; }
}
