using System;
using ActiveLogin.Authentication.BankId.AspNetCore.Azure.KeyVault;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Azure
{
    public static class BankIdAuthenticationBuilderAzureExtensions
    {
        public static IBankIdAuthenticationBuilder UseClientCertificateFromAzureKeyVault(this IBankIdAuthenticationBuilder builder, IConfigurationSection configurationSection)
        {
            builder.AuthenticationBuilder.Services.Configure<ClientCertificateFromAzureKeyVaultOptions>(configurationSection.Bind);

            return UseClientCertificateFromAzureKeyVault(builder);
        }

        public static IBankIdAuthenticationBuilder UseClientCertificateFromAzureKeyVault(this IBankIdAuthenticationBuilder builder, Action<ClientCertificateFromAzureKeyVaultOptions> configureOptions)
        {
            builder.AuthenticationBuilder.Services.Configure(configureOptions);

            return UseClientCertificateFromAzureKeyVault(builder);
        }

        public static IBankIdAuthenticationBuilder UseClientCertificateFromAzureKeyVault(this IBankIdAuthenticationBuilder builder)
        {
            builder.UseClientCertificate(() =>
            {
                var options = builder.AuthenticationBuilder.Services
                    .BuildServiceProvider()
                    .GetService<IOptions<ClientCertificateFromAzureKeyVaultOptions>>();

                using (var keyVaultCertificateClient = AzureKeyVaultCertificateClient.Create(options.Value))
                {
                    return keyVaultCertificateClient.GetX509Certificate2Async(options.Value.AzureKeyVaultSecretIdentifier)
                        .ConfigureAwait(false)
                        .GetAwaiter()
                        .GetResult();
                }
            });

            return builder;
        }
    }
}
