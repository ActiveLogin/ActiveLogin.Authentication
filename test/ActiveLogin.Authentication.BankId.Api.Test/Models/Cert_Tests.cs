using System;
using ActiveLogin.Authentication.BankId.Api.Models;
using Xunit;

namespace ActiveLogin.Authentication.BankId.Api.Test.Models
{
    public class Cert_Tests
    {
        [Fact]
        public async void Cert_Constructor_ShoulAllowSettingManualDates()
        {
            // Arrange
            var notBefore = new DateTime(2018, 12, 24, 15, 00, 00, DateTimeKind.Utc);
            var notAfter = new DateTime(2018, 12, 25, 15, 00, 00, DateTimeKind.Utc);

            // Act
            var cert = new Cert(notBefore, notAfter);

            // Assert
            Assert.Equal(notBefore, cert.NotBefore);
            Assert.Equal(notAfter, cert.NotAfter);
        }
    }
}
