using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Azure
{
    public static class BankIdAuthenticationBuilderAzureExtensions
    {
        public static IBankIdAuthenticationBuilder UseBankIdClientCertificateFromAzureKeyVault(this IBankIdAuthenticationBuilder builder, IConfigurationSection configurationSection)
        {
            builder.AuthenticationBuilder.Services.Configure<ClientCertificateFromAzureKeyVaultOptions>(configurationSection.Bind);

            return UseBankIdClientCertificateFromAzureKeyVault(builder);
        }

        public static IBankIdAuthenticationBuilder UseBankIdClientCertificateFromAzureKeyVault(this IBankIdAuthenticationBuilder builder, Action<ClientCertificateFromAzureKeyVaultOptions> configureOptions)
        {
            builder.AuthenticationBuilder.Services.Configure(configureOptions);

            return UseBankIdClientCertificateFromAzureKeyVault(builder);
        }

        public static IBankIdAuthenticationBuilder UseBankIdClientCertificateFromAzureKeyVault(this IBankIdAuthenticationBuilder builder)
        {
            builder.UseBankIdClientCertificate(() =>
            {
                var options = builder.AuthenticationBuilder.Services.BuildServiceProvider().GetService<IOptions<ClientCertificateFromAzureKeyVaultOptions>>();
                using (var keyVaultCertificateClient = new AzureKeyVaultCertificateClient(options.Value.AzureAdClientId, options.Value.AzureAdClientSecret))
                {
                    return keyVaultCertificateClient.GetX509Certificate2Async(options.Value.AzureKeyVaultSecretIdentifier).ConfigureAwait(false).GetAwaiter().GetResult();
                }
            });

            return builder;
        }
    }
}
