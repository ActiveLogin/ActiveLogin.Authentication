using System;

using ActiveLogin.Authentication.BankId.Core.CertificatePolicies;
using Xunit;

namespace ActiveLogin.Authentication.BankId.Core.Test.CertificatePolicies;

public class BankIdCertificatePolicyResolverForProduction_Tests
{
    private readonly BankIdCertificatePolicyResolverForProduction _resolver;

    public BankIdCertificatePolicyResolverForProduction_Tests()
    {
        _resolver = new BankIdCertificatePolicyResolverForProduction();
    }

    [Fact]
    public void Test_Policy_Is_Not_Present()
    {
        Assert.Throws<NotSupportedException>(() =>
        {
            var policy = _resolver.Resolve(BankIdCertificatePolicy.TestBankId);
        });
    }

    [Fact]
    public void Resolves_BankIdOnFile()
    {
        var policy = _resolver.Resolve(BankIdCertificatePolicy.BankIdOnFile);
        Assert.Equal("1.2.752.78.1.1", policy);
    }

    [Fact]
    public void Resolves_BankIdOnSmartCard()
    {
        var policy = _resolver.Resolve(BankIdCertificatePolicy.BankIdOnSmartCard);
        Assert.Equal("1.2.752.78.1.2", policy);

    }

    [Fact]
    public void Resolves_MobileBankId()
    {
        var policy = _resolver.Resolve(BankIdCertificatePolicy.MobileBankId);
        Assert.Equal("1.2.752.78.1.5", policy);
    }
}
