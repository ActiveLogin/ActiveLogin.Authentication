using System;
using ActiveLogin.Authentication.BankId.AspNetCore;
using ActiveLogin.Authentication.BankId.AspNetCore.Azure;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BankIdBuilderAzureExtensions
    {
        public static IBankIdBuilder UseClientCertificateFromAzureKeyVault(this IBankIdBuilder builder, IConfigurationSection configurationSection)
        {
            var options = new ClientCertificateFromAzureKeyVaultOptions();
            configurationSection.Bind(options);
            return UseClientCertificateFromAzureKeyVault(builder, options);
        }

        public static IBankIdBuilder UseClientCertificateFromAzureKeyVault(this IBankIdBuilder builder, Action<ClientCertificateFromAzureKeyVaultOptions> configureOptions)
        {
            var options = new ClientCertificateFromAzureKeyVaultOptions();
            configureOptions(options);
            return UseClientCertificateFromAzureKeyVault(builder, options);
        }

        public static IBankIdBuilder UseClientCertificateFromAzureKeyVault(this IBankIdBuilder builder, ClientCertificateFromAzureKeyVaultOptions options)
        {
            if (string.IsNullOrEmpty(options.AzureKeyVaultSecretKey))
            {
                throw new ArgumentNullException(nameof(options.AzureKeyVaultSecretKey));
            }

            builder.UseClientCertificate(() =>
            {
                var keyVaultCertificateClient = AzureKeyVaultCertificateClient.Create(options);

                return keyVaultCertificateClient.GetX509Certificate2(options.AzureKeyVaultSecretKey);
            });

            return builder;
        }
    }
}
