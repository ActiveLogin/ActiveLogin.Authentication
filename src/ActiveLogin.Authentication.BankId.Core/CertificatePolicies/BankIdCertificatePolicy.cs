namespace ActiveLogin.Authentication.BankId.Core.CertificatePolicies;

public enum BankIdCertificatePolicy
{
    /// <summary>
    /// BankID on file.
    /// </summary>
    BankIdOnFile,

    /// <summary>
    /// BankID on smartcard.
    /// </summary>
    BankIdOnSmartCard,

    /// <summary>
    /// Mobile BankID.
    /// </summary>
    MobileBankId,

    /// <summary>
    /// Test BankID for some BankID Banks.
    /// </summary>
    TestBankId
}
