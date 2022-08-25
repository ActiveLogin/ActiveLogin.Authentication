namespace ActiveLogin.Authentication.BankId.Core.Models;

public class BankIdFlowOptions
{
    public BankIdFlowOptions(
        List<string> certificatePolicies,
        bool sameDevice,
        bool allowBiometric)
    {
        CertificatePolicies = certificatePolicies;
        SameDevice = sameDevice;
        AllowBiometric = allowBiometric;
    }

    public List<string> CertificatePolicies { get; }
        
    public bool SameDevice { get; }

    public bool AllowBiometric { get; }
}
