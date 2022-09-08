using System;

using Xunit;

namespace ActiveLogin.Authentication.BankId.Core.Test;

public class BankIdCertificates_Tests
{
    private readonly TimeSpan TimeBeforeExpire = TimeSpan.FromDays(-14);

    [Fact]
    public void BankIdApiClientCertificateTest_Should_Not_Be_Expired()
    {
        var cert = BankIdCertificates.BankIdApiClientCertificateTest;
        var certExpires = cert.NotAfter;
        var now = DateTime.UtcNow;

        Assert.True(now <= certExpires.Add(TimeBeforeExpire));
    }

    [Fact]
    public void BankIdApiRootCertificateProd_Should_Not_Be_Expired()
    {
        var cert = BankIdCertificates.BankIdApiRootCertificateProd;
        var certExpires = cert.NotAfter;
        var now = DateTime.UtcNow;

        Assert.True(now <= certExpires.Add(TimeBeforeExpire));
    }

    [Fact]
    public void BankIdApiRootCertificateTest_Should_Not_Be_Expired()
    {
        var cert = BankIdCertificates.BankIdApiRootCertificateTest;
        var certExpires = cert.NotAfter;
        var now = DateTime.UtcNow;

        Assert.True(now <= certExpires.Add(TimeBeforeExpire));
    }
}
