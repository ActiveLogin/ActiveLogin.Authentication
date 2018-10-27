namespace ActiveLogin.Authentication.BankId.Api.CertificatePolicies
{
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
        /// Nordea e-id on file and on smart card.
        /// </summary>
        NordeaEidOnFileAndOnSmartCard,

        /// <summary>
        /// Test BankID for some BankID Banks.
        /// </summary>
        TestBankId
    }
}
