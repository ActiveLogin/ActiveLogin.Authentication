using ActiveLogin.Authentication.BankId.Api.CertificatePolicies;
using Xunit;

namespace ActiveLogin.Authentication.BankId.Api.Test.CertificatePolicies
{
    public class BankIdCertificatePolicyBuilder_Tests
    {
        [Fact]
        public void Can_Build_Single_Policy_From_Enum()
        {
            var policies = BankIdCertificatePolicyBuilder
                                        .Create()
                                        .Add(BankIdCertificatePolicy.BankIdOnFile)
                                        .BuildForProductionEnvironment();

            Assert.Equal(new [] { "1.2.752.78.1.1" }, policies);
        }

        [Fact]
        public void Can_Build_Multiple_Policies_From_Enum()
        {
            var policies = BankIdCertificatePolicyBuilder
                .Create()
                .Add(BankIdCertificatePolicy.BankIdOnFile)
                .Add(BankIdCertificatePolicy.MobileBankId)
                .BuildForProductionEnvironment();

            Assert.Equal(new[] { "1.2.752.78.1.1", "1.2.752.78.1.5" }, policies);
        }

        [Fact]
        public void Can_Build_Multiple_Policies_From_Enum_And_String()
        {
            var policies = BankIdCertificatePolicyBuilder
                .Create()
                .Add(BankIdCertificatePolicy.BankIdOnFile)
                .Add(BankIdCertificatePolicy.MobileBankId)
                .Add("POLICY")
                .BuildForProductionEnvironment();

            Assert.Equal(new[] { "1.2.752.78.1.1", "1.2.752.78.1.5", "POLICY" }, policies);
        }

        [Fact]
        public void Test_Policy_Is_Not_Present_For_Production()
        {
            var policies = BankIdCertificatePolicyBuilder
                .Create()
                .Add(BankIdCertificatePolicy.TestBankId)
                .BuildForProductionEnvironment();

            Assert.Equal(new string[]{ }, policies);
        }

        [Fact]
        public void Test_Policy_Is_Present_For_Production()
        {
            var policies = BankIdCertificatePolicyBuilder
                .Create()
                .Add(BankIdCertificatePolicy.TestBankId)
                .BuildForTestEnvironment();

            Assert.Equal(new[] { "1.2.752.60.1.6" }, policies);
        }
    }
}
