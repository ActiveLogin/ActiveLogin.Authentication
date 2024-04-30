namespace ActiveLogin.Authentication.BankId.Core.CertificatePolicies;

public class BankIdCertificatePolicyResolverForTest : IBankIdCertificatePolicyResolver
{
    public const string BankIdOnFileTestEnvironment = "1.2.3.4.5";
    public const string BankIdOnSmartCardTestEnvironment = "1.2.3.4.10";
    public const string MobileBankIdTestEnvironment = "1.2.3.4.25";
    public const string TestBankIdTestEnvironment = "1.2.752.60.1.6";
    
    public string Resolve(BankIdCertificatePolicy certificatePolicy)
    {
        return certificatePolicy switch
        {
            BankIdCertificatePolicy.BankIdOnFile => BankIdOnFileTestEnvironment,
            BankIdCertificatePolicy.BankIdOnSmartCard => BankIdOnSmartCardTestEnvironment,
            BankIdCertificatePolicy.MobileBankId => MobileBankIdTestEnvironment,
            BankIdCertificatePolicy.TestBankId => TestBankIdTestEnvironment,

            _ => string.Empty,
        };
    }
}
