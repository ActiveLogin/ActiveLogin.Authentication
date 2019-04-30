namespace ActiveLogin.Authentication.BankId.Api.CertificatePolicies
{
    internal static class BankIdCertificatePolicyConstants
    {
        public const string BankIdOnFileProductionEnvironment = "1.2.752.78.1.1";
        public const string BankIdOnFileTestEnvironment = "1.2.3.4.5";

        public const string BankIdOnSmartCardProductionEnvironment = "1.2.752.78.1.2";
        public const string BankIdOnSmartCardTestEnvironment = "1.2.3.4.10";

        public const string MobileBankIdProductionEnvironment = "1.2.752.78.1.5";
        public const string MobileBankIdTestEnvironment = "1.2.3.4.25";

        public const string NordeaEidOnFileAndOnSmartCardProductionEnvironment = "1.2.752.71.1.3";
        public const string NordeaEidOnFileAndOnSmartCardTestEnvironment = "1.2.752.71.1.3";

        public const string TestBankIdTestEnvironment = "1.2.752.60.1.6";
    }
}