namespace ActiveLogin.Authentication.BankId.Api.CertificatePolicies;

public static class BankIdCertificatePolicies
{
    public static List<string> GetPoliciesForProductionEnvironment(params BankIdCertificatePolicy[] certificatePolicies)
    {
        return GetPolicies(certificatePolicies, GetPolicyForProductionEnvironment);
    }

    public static List<string> GetPoliciesForTestEnvironment(params BankIdCertificatePolicy[] certificatePolicies)
    {
        return GetPolicies(certificatePolicies, GetPolicyForTestEnvironment);
    }

    private static List<string> GetPolicies(BankIdCertificatePolicy[] certificatePolicies, Func<BankIdCertificatePolicy, string> getPolicy)
    {
        return certificatePolicies?.Select(getPolicy).Where(x => !string.IsNullOrEmpty(x)).ToList() ?? new List<string>();
    }

    public static string GetPolicyForProductionEnvironment(BankIdCertificatePolicy certificatePolicy)
    {
        return certificatePolicy switch
        {
            BankIdCertificatePolicy.BankIdOnFile => BankIdCertificatePolicyConstants.BankIdOnFileProductionEnvironment,
            BankIdCertificatePolicy.BankIdOnSmartCard => BankIdCertificatePolicyConstants.BankIdOnSmartCardProductionEnvironment,
            BankIdCertificatePolicy.MobileBankId => BankIdCertificatePolicyConstants.MobileBankIdProductionEnvironment,

            _ => string.Empty,
        };
    }

    public static string GetPolicyForTestEnvironment(BankIdCertificatePolicy certificatePolicy)
    {
        return certificatePolicy switch
        {
            BankIdCertificatePolicy.BankIdOnFile => BankIdCertificatePolicyConstants.BankIdOnFileTestEnvironment,
            BankIdCertificatePolicy.BankIdOnSmartCard => BankIdCertificatePolicyConstants.BankIdOnSmartCardTestEnvironment,
            BankIdCertificatePolicy.MobileBankId => BankIdCertificatePolicyConstants.MobileBankIdTestEnvironment,
            BankIdCertificatePolicy.TestBankId => BankIdCertificatePolicyConstants.TestBankIdTestEnvironment,

            _ => string.Empty,
        };
    }
}
