namespace ActiveLogin.Authentication.BankId.Core.Models;

public class BankIdFlowOptions
{
    public BankIdFlowOptions(
        List<string> certificatePolicies,
        bool sameDevice,
        bool requirePinCode)
    {
        CertificatePolicies = certificatePolicies;
        SameDevice = sameDevice;
        RequirePinCode = requirePinCode;
    }

    public List<string> CertificatePolicies { get; }
        
    public bool SameDevice { get; }

    public bool RequirePinCode { get; }
}
