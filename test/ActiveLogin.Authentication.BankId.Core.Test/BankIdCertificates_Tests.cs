using System;

using Xunit;

namespace ActiveLogin.Authentication.BankId.Core.Test;

public class BankIdCertificates_Tests
{
    private readonly TimeSpan TimeBeforeExpire = TimeSpan.FromDays(-14);

    [Fact]
    public void BankIdApiClientCertificateTestP12_Should_Not_Be_Expired()
    {
        var cert = BankIdCertificates.GetBankIdApiClientCertificateTest(Certificate.TestCertificateFormat.P12);
        var certExpires = cert.NotAfter;
        var now = DateTime.UtcNow;

        Assert.True(now <= certExpires.Add(TimeBeforeExpire));
    }

    [Fact]
    public void BankIdApiClientCertificateTestPEM_Should_Not_Be_Expired()
    {
        var cert = BankIdCertificates.GetBankIdApiClientCertificateTest(Certificate.TestCertificateFormat.PEM);
        var certExpires = cert.NotAfter;
        var now = DateTime.UtcNow;

        Assert.True(now <= certExpires.Add(TimeBeforeExpire));
    }

    [Fact]
    public void BankIdApiClientCertificateTestPfx_Should_Not_Be_Expired()
    {
        var cert = BankIdCertificates.GetBankIdApiClientCertificateTest(Certificate.TestCertificateFormat.PFX);
        var certExpires = cert.NotAfter;
        var now = DateTime.UtcNow;

        Assert.True(now <= certExpires.Add(TimeBeforeExpire));
    }

    [Fact]
    public void BankIdApiRootCertificateProd_Should_Not_Be_Expired()
    {
        var cert = BankIdCertificates.GetBankIdApiRootCertificateProd();
        var certExpires = cert.NotAfter;
        var now = DateTime.UtcNow;

        Assert.True(now <= certExpires.Add(TimeBeforeExpire));
    }

    [Fact]
    public void BankIdApiRootCertificateTest_Should_Not_Be_Expired()
    {
        var cert = BankIdCertificates.GetBankIdApiRootCertificateTest();
        var certExpires = cert.NotAfter;
        var now = DateTime.UtcNow;

        Assert.True(now <= certExpires.Add(TimeBeforeExpire));
    }
}
