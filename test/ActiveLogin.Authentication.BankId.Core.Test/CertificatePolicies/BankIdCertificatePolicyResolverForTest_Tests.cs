using System;

using ActiveLogin.Authentication.BankId.Core.CertificatePolicies;
using Xunit;

namespace ActiveLogin.Authentication.BankId.Core.Test.CertificatePolicies;

public class BankIdCertificatePolicyResolverForTest_Tests
{
    private readonly BankIdCertificatePolicyResolverForTest _resolver;

    public BankIdCertificatePolicyResolverForTest_Tests()
    {
        _resolver = new BankIdCertificatePolicyResolverForTest();
    }

    [Fact]
    public void Resolves_BankIdOnFile()
    {
        var policy = _resolver.Resolve(BankIdCertificatePolicy.BankIdOnFile);
        Assert.Equal("1.2.3.4.5", policy);
    }

    [Fact]
    public void Resolves_BankIdOnSmartCard()
    {
        var policy = _resolver.Resolve(BankIdCertificatePolicy.BankIdOnSmartCard);
        Assert.Equal("1.2.3.4.10", policy);

    }

    [Fact]
    public void Resolves_MobileBankId()
    {
        var policy = _resolver.Resolve(BankIdCertificatePolicy.MobileBankId);
        Assert.Equal("1.2.3.4.25", policy);
    }

    [Fact]
    public void Resolves_TestBankId()
    {
        var policy = _resolver.Resolve(BankIdCertificatePolicy.TestBankId);
        Assert.Equal("1.2.752.60.1.6", policy);
    }
}
