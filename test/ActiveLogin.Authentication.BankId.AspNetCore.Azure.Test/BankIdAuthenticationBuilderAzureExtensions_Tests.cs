using System;
using System.Collections.Generic;
using ActiveLogin.Authentication.BankId.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Azure.Test;

public class BankIdBuilderAzureExtension
{
    public class UseClientCertificateFromAzureKeyVault
    {
        private ServiceProvider SetupTest(IDictionary<string, string> config)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .Add(new MemoryConfigurationSource { InitialData = config })
                .Build();

            var collection = new ServiceCollection();

            collection.AddBankId(bankId =>
            {
                bankId.UseTestEnvironment()
                      .UseClientCertificateFromAzureKeyVault(configuration.GetSection("ActiveLogin:BankId:ClientCertificate"));
            });

            return collection.BuildServiceProvider();
        }

        [Fact]
        public void With_ManagedIdentity_With_MalformedSecretIdentifier__Throws()
        {
            var config = new Dictionary<string, string>
            {
                { "ActiveLogin:BankId:ClientCertificate:AzureKeyVaultUri", "someuri" },
                { "ActiveLogin:BankId:ClientCertificate:AzureKeyVaultSecretName", "somename" },
                { "ActiveLogin:BankId:ClientCertificate:UseManagedIdentity", "true" }
            };

            var provider = SetupTest(config);

            Assert.Throws<UriFormatException>(() => provider.GetService<IBankIdApiClient>());
        }

        [Fact]
        public void Without_SecretIdentifier__Throws()
        {
            var config = new Dictionary<string, string>
            {
                {"ActiveLogin:BankId:ClientCertificate:AzureKeyVaultSecretName", ""},
            };

            var exception = Assert.Throws<ArgumentException>(() =>
            {
                var collection = new ServiceCollection();

                collection.AddBankId(bankId =>
                {
                    bankId.UseTestEnvironment()
                        .UseClientCertificateFromAzureKeyVault(new ClientCertificateFromAzureKeyVaultOptions());
                });
            });

            Assert.Contains("AzureKeyVaultSecretName", exception.Message);
        }
    }
}
