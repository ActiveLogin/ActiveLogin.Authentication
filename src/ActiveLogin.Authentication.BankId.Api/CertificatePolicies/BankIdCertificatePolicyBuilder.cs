using System.Collections.Generic;

namespace ActiveLogin.Authentication.BankId.Api.CertificatePolicies
{
    public class BankIdCertificatePolicyBuilder
    {
        private readonly List<string> _policiesForTestEnvironment = new List<string>();
        private readonly List<string> _policiesForProductionEnvironment = new List<string>();

        private BankIdCertificatePolicyBuilder()
        {

        }

        public static BankIdCertificatePolicyBuilder Create()
        {
            return new BankIdCertificatePolicyBuilder();
        }

        public BankIdCertificatePolicyBuilder Add(string certificatePolicy)
        {
            Add(certificatePolicy, certificatePolicy);
            return this;
        }

        public BankIdCertificatePolicyBuilder Add(BankIdCertificatePolicy certificatePolicy)
        {
            var policiyForProductionEnvironment = GetPoliciyForProductionEnvironment(certificatePolicy);
            var policiyForTestEnvironment = GetPoliciyForTestEnvironment(certificatePolicy);

            Add(policiyForProductionEnvironment, policiyForTestEnvironment);

            return this;
        }

        private string GetPoliciyForProductionEnvironment(BankIdCertificatePolicy certificatePolicy)
        {
            switch (certificatePolicy)
            {
                case BankIdCertificatePolicy.BankIdOnFile:
                    return BankIdCertificatePolicyConstants.BankIdOnFileProductionEnvironment;
                case BankIdCertificatePolicy.BankIdOnSmartCard:
                    return BankIdCertificatePolicyConstants.BankIdOnSmartCardProductionEnvironment;
                case BankIdCertificatePolicy.MobileBankId:
                    return BankIdCertificatePolicyConstants.MobileBankIdProductionEnvironment;
                case BankIdCertificatePolicy.NordeaEidOnFileAndOnSmartCard:
                    return BankIdCertificatePolicyConstants.NordeaEidOnFileAndOnSmartCardProductionEnvironment;
            }

            return null;
        }

        private string GetPoliciyForTestEnvironment(BankIdCertificatePolicy certificatePolicy)
        {
            switch (certificatePolicy)
            {
                case BankIdCertificatePolicy.BankIdOnFile:
                    return BankIdCertificatePolicyConstants.BankIdOnFileTestEnvironment;
                case BankIdCertificatePolicy.BankIdOnSmartCard:
                    return BankIdCertificatePolicyConstants.BankIdOnSmartCardTestEnvironment;
                case BankIdCertificatePolicy.MobileBankId:
                    return BankIdCertificatePolicyConstants.MobileBankIdTestEnvironment;
                case BankIdCertificatePolicy.NordeaEidOnFileAndOnSmartCard:
                    return BankIdCertificatePolicyConstants.NordeaEidOnFileAndOnSmartCardTestEnvironment;
                case BankIdCertificatePolicy.TestBankId:
                    return BankIdCertificatePolicyConstants.TestBankIdTestEnvironment;
            }

            return null;
        }

        private void Add(string policiyForProductionEnvironment, string policiyForTestEnvironment)
        {
            if (!string.IsNullOrEmpty(policiyForProductionEnvironment))
            {
                _policiesForProductionEnvironment.Add(policiyForProductionEnvironment);
            }

            if (!string.IsNullOrEmpty(policiyForTestEnvironment))
            {
                _policiesForTestEnvironment.Add(policiyForTestEnvironment);
            }
        }

        public List<string> BuildForProductionEnvironment()
        {
            return _policiesForProductionEnvironment;
        }

        public List<string> BuildForTestEnvironment()
        {
            return _policiesForTestEnvironment;
        }
    }

    internal class BankIdCertificatePolicyConstants
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
