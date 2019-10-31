using ActiveLogin.Authentication.BankId.Api;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Xunit;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Azure.Test
{
    public class BankIdAuthenticationBuilderAzureExtension
    {
        public class UseClientCertificateFromAzureKeyVault
        {
            private ServiceProvider SetupTest(IDictionary<string, string> config)
            {
                IConfiguration configuration = new ConfigurationBuilder()
                    .Add(new MemoryConfigurationSource { InitialData = config })
                    .Build();

                var collection = new ServiceCollection();
                _ = new AuthenticationBuilder(collection)
                    .AddBankId(builder =>
                    {
                        builder.UseTestEnvironment()
                            .UseClientCertificateFromAzureKeyVault(configuration.GetSection("ActiveLogin:BankId:ClientCertificate"));
                    });

                return collection.BuildServiceProvider();
            }

            [Fact]
            public void With_ManagedIdentity_With_MalformedSecretIdentifier__Throws()
            {
                var config = new Dictionary<string, string>
                {
                    { "ActiveLogin:BankId:ClientCertificate:AzureKeyVaultIdentifier", "somesecret" },
                    { "ActiveLogin:BankId:ClientCertificate:UseManagedIdentity", "true" }
                };

                var provider = SetupTest(config);

                Assert.Throws<ArgumentException>(() => provider.GetService<IBankIdApiClient>());
            }

            [Fact]
            public void With_ManagedIdentity_With_MalformedCertificateIdentifier__Throws()
            {
                var config = new Dictionary<string, string>
                {
                    { "ActiveLogin:BankId:ClientCertificate:AzureKeyVaultIdentifier", "somecertificate" },
                    { "ActiveLogin:BankId:ClientCertificate:UseManagedIdentity", "true" }
                };

                var provider = SetupTest(config);

                Assert.Throws<ArgumentException>(() => provider.GetService<IBankIdApiClient>());
            }

            [Fact]
            public void Without_SecretIdentifier__Throws()
            {
                var config = new Dictionary<string, string>
                {
                    { "ActiveLogin:BankId:ClientCertificate:AzureKeyVaultIdentifier", "" },
                };

                var provider = SetupTest(config);

                ArgumentException exception = Assert.Throws<ArgumentException>(() => provider.GetService<IBankIdApiClient>());
                Assert.Contains("AzureKeyVaultIdentifier", exception.Message);
            }

            [Fact]
            public void Without_UseManagedIdentity__Without_ClientId__Throws()
            {
                var config = new Dictionary<string, string>
                {
                    { "ActiveLogin:BankId:ClientCertificate:AzureAdClientId", "" },
                    { "ActiveLogin:BankId:ClientCertificate:AzureAdClientSecret", "clientsecret" },
                    { "ActiveLogin:BankId:ClientCertificate:AzureKeyVaultIdentifier", "keysecret" },
                    { "ActiveLogin:BankId:ClientCertificate:UseManagedIdentity", "false" }
                };

                var provider = SetupTest(config);

                ArgumentException exception = Assert.Throws<ArgumentException>(() => provider.GetService<IBankIdApiClient>());
                Assert.Contains("AzureAdClientId", exception.Message);
            }

            [Fact]
            public void Without_UseManagedIdentity__Without_AzureAdClientSecret__Throws()
            {
                var config = new Dictionary<string, string>
                {
                    { "ActiveLogin:BankId:ClientCertificate:AzureAdClientId", "id" },
                    { "ActiveLogin:BankId:ClientCertificate:AzureAdClientSecret", "" },
                    { "ActiveLogin:BankId:ClientCertificate:AzureKeyVaultIdentifier", "keysecret" },
                    { "ActiveLogin:BankId:ClientCertificate:UseManagedIdentity", "false" }
                };

                var provider = SetupTest(config);

                ArgumentException exception = Assert.Throws<ArgumentException>(() => provider.GetService<IBankIdApiClient>());
                Assert.Contains("AzureAdClientSecret", exception.Message);
            }
        }
    }
}
