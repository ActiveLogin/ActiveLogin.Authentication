using System;
using ActiveLogin.Authentication.BankId.AspNetCore;
using ActiveLogin.Authentication.BankId.AspNetCore.Azure;
using ActiveLogin.Authentication.BankId.AspNetCore.Azure.KeyVault;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BankIdBuilderAzureExtensions
    {
        public static IBankIdBuilder UseClientCertificateFromAzureKeyVault(this IBankIdBuilder builder, IConfigurationSection configurationSection)
        {
            builder.AuthenticationBuilder.Services.Configure<ClientCertificateFromAzureKeyVaultOptions>(configurationSection.Bind);

            return UseClientCertificateFromAzureKeyVault(builder);
        }

        public static IBankIdBuilder UseClientCertificateFromAzureKeyVault(this IBankIdBuilder builder, Action<ClientCertificateFromAzureKeyVaultOptions> configureOptions)
        {
            builder.AuthenticationBuilder.Services.Configure(configureOptions);

            return UseClientCertificateFromAzureKeyVault(builder);
        }

        public static IBankIdBuilder UseClientCertificateFromAzureKeyVault(this IBankIdBuilder builder)
        {
            builder.UseClientCertificate(() =>
            {
                var options = builder.AuthenticationBuilder.Services
                    .BuildServiceProvider()
                    .GetService<IOptions<ClientCertificateFromAzureKeyVaultOptions>>();

                using (var keyVaultCertificateClient = AzureKeyVaultCertificateClient.Create(options.Value))
                {
                    if (string.IsNullOrEmpty(options.Value.AzureKeyVaultSecretIdentifier))
                    {
                        throw new ArgumentNullException(nameof(options.Value.AzureKeyVaultSecretIdentifier));
                    }

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
