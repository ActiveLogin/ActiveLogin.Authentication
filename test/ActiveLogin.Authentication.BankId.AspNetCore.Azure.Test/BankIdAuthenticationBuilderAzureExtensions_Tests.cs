using ActiveLogin.Authentication.BankId.AspNetCore.Azure.KeyVault;
using Microsoft.Azure.KeyVault;
using Newtonsoft.Json;
using System;
using Xunit;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Azure.Test
{
    public class BankIdAuthenticationBuilderAzureExtensions_Tests
    {
        [Fact]
        public void Factory__Without_SecretIdentifier__Throws()
        {
            var options = new ClientCertificateFromAzureKeyVaultOptions
            {
                AzureKeyVaultSecretIdentifier = "keysecret",
            };
            Assert.Throws<ArgumentException>(() => AzureKeyVaultCertificateClient.Create(options));
        }

        [Fact]
        public void Factory__Without_UseManagedIdentity__Without_ClientId__Throws()
        {
            var options = new ClientCertificateFromAzureKeyVaultOptions
            {
                AzureAdClientId = "id",
                AzureKeyVaultSecretIdentifier = "keysecret",
                UseManagedIdentity = false
            };
            Assert.Throws<ArgumentException>(() => AzureKeyVaultCertificateClient.Create(options));
        }

        [Fact]
        public void Factory__Without_UseManagedIdentity__Without_AzureAdClientSecret__Throws()
        {
            var options = new ClientCertificateFromAzureKeyVaultOptions
            {
                AzureAdClientSecret = "secret",
                AzureKeyVaultSecretIdentifier = "keysecret",
                UseManagedIdentity = false
            };
            Assert.Throws<ArgumentException>(() => AzureKeyVaultCertificateClient.Create(options));
        }
    }
}
