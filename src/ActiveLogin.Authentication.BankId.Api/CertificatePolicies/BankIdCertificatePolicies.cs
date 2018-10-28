using System;
using System.Collections.Generic;
using System.Linq;

namespace ActiveLogin.Authentication.BankId.Api.CertificatePolicies
{
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

            return string.Empty;
        }

        public static string GetPolicyForTestEnvironment(BankIdCertificatePolicy certificatePolicy)
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

            return string.Empty;
        }
    }
}
