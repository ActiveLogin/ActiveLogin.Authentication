namespace ActiveLogin.Authentication.BankId.AspNetCore.Models;

public class BankIdUiOptions
{
    public BankIdUiOptions(
        List<string> certificatePolicies,
        bool sameDevice,
        bool requirePinCode,
        string cancelReturnUrl,
        string stateCookieName)
    {
        CertificatePolicies = certificatePolicies;
        SameDevice = sameDevice;
        RequirePinCode = requirePinCode;
        CancelReturnUrl = cancelReturnUrl;
        StateCookieName = stateCookieName;
    }

    public List<string> CertificatePolicies { get; }

    public bool SameDevice { get; }

    public bool RequirePinCode { get; }

    public string CancelReturnUrl { get; }

    public string StateCookieName { get; }
}
