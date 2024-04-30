namespace ActiveLogin.Authentication.BankId.Core.CertificatePolicies;

public class BankIdCertificatePolicyResolverForProduction : IBankIdCertificatePolicyResolver
{
    public const string BankIdOnFileProductionEnvironment = "1.2.752.78.1.1";
    public const string BankIdOnSmartCardProductionEnvironment = "1.2.752.78.1.2";
    public const string MobileBankIdProductionEnvironment = "1.2.752.78.1.5";

    public string Resolve(BankIdCertificatePolicy certificatePolicy)
    {
        return certificatePolicy switch
        {
            BankIdCertificatePolicy.BankIdOnFile => BankIdOnFileProductionEnvironment,
            BankIdCertificatePolicy.BankIdOnSmartCard => BankIdOnSmartCardProductionEnvironment,
            BankIdCertificatePolicy.MobileBankId => MobileBankIdProductionEnvironment,
            BankIdCertificatePolicy.TestBankId => throw new NotSupportedException("Test BankID is not supported in production."),

            _ => throw new ArgumentOutOfRangeException(nameof(certificatePolicy), certificatePolicy, "Unknown certificate policy")
        };
    }
}
