using System.Collections.Generic;
using ActiveLogin.Authentication.BankId.Api.CertificatePolicies;
using Xunit;

namespace ActiveLogin.Authentication.BankId.Api.Test.CertificatePolicies
{
    public class BankIdCertificatePolicyBuilder_Tests
    {
        [Fact]
        public void Can_Build_Multiple_Policies_From_Enum()
        {
            List<string> policies = BankIdCertificatePolicies.GetPoliciesForProductionEnvironment(
                BankIdCertificatePolicy.BankIdOnFile,
                BankIdCertificatePolicy.MobileBankId
            );

            Assert.Equal(new[] { "1.2.752.78.1.1", "1.2.752.78.1.5" }, policies);
        }

        [Fact]
        public void Can_Build_Single_Policy_From_Enum()
        {
            string policy = BankIdCertificatePolicies.GetPolicyForProductionEnvironment(BankIdCertificatePolicy.BankIdOnFile);

            Assert.Equal("1.2.752.78.1.1", policy);
        }

        [Fact]
        public void Test_Policy_Is_Not_Present_For_Production()
        {
            List<string> policies = BankIdCertificatePolicies.GetPoliciesForProductionEnvironment(BankIdCertificatePolicy.TestBankId);

            Assert.Equal(new string[] { }, policies);
        }

        [Fact]
        public void Test_Policy_Is_Present_For_Test()
        {
            List<string> policies = BankIdCertificatePolicies.GetPoliciesForTestEnvironment(BankIdCertificatePolicy.TestBankId);

            Assert.Equal(new[] { "1.2.752.60.1.6" }, policies);
        }
    }
}
