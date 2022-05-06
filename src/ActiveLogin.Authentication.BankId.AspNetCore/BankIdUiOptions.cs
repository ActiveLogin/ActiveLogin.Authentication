using ActiveLogin.Authentication.BankId.Core.Models;

namespace ActiveLogin.Authentication.BankId;

public class BankIdUiOptions
{
    public BankIdUiOptions(
        List<string> certificatePolicies,
        bool sameDevice,
        bool allowBiometric,
        string cancelReturnUrl,
        string stateCookieName)
    {
        CertificatePolicies = certificatePolicies;
        SameDevice = sameDevice;
        AllowBiometric = allowBiometric;
        CancelReturnUrl = cancelReturnUrl;
        StateCookieName = stateCookieName;
    }

    public List<string> CertificatePolicies { get; }

    public bool SameDevice { get; }

    public bool AllowBiometric { get; }

    public string CancelReturnUrl { get; }

    public string StateCookieName { get; }
}

public static class BankUiOptionsExtensions
{
    public static BankIdFlowOptions ToBankIdFlowOptions(this BankIdUiOptions options) => new(
        options.CertificatePolicies,
        options.SameDevice,
        options.AllowBiometric,
        options.CancelReturnUrl,
        options.StateCookieName);
}
