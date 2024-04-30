namespace ActiveLogin.Authentication.BankId.Core.CertificatePolicies;

public interface IBankIdCertificatePolicyResolver
{
    public string Resolve(BankIdCertificatePolicy certificatePolicy);
}
