using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using ActiveLogin.Authentication.BankId.AspNetCore.Azure.KeyVault;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;
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
                var options = builder.AuthenticationBuilder.Services.BuildServiceProvider().GetService<IOptions<ClientCertificateFromAzureKeyVaultOptions>>();
                using (var keyVaultCertificateClient = new AzureKeyVaultCertificateClient(options.Value.AzureAdClientId, options.Value.AzureAdClientSecret))
                {
                    return keyVaultCertificateClient.GetX509Certificate2Async(options.Value.AzureKeyVaultSecretIdentifier).ConfigureAwait(false).GetAwaiter().GetResult();
                }
            });

            return builder;
        }

        public static IBankIdAuthenticationBuilder UseClientCertificateFromManagedIdentityAzureKeyVaultOptions(this IBankIdAuthenticationBuilder builder, IConfigurationSection configurationSection)
        {
            builder.AuthenticationBuilder.Services.Configure<ClientCertificateFromManagedIdentityAzureKeyVaultOptions>(configurationSection.Bind);

            return UseClientCertificateFromManagedIdentityAzureKeyVaultOptions(builder);
        }

        public static IBankIdAuthenticationBuilder UseClientCertificateFromManagedIdentityAzureKeyVaultOptions(this IBankIdAuthenticationBuilder builder, Action<ClientCertificateFromManagedIdentityAzureKeyVaultOptions> configureOptions)
        {
            builder.AuthenticationBuilder.Services.Configure(configureOptions);

            return UseClientCertificateFromManagedIdentityAzureKeyVaultOptions(builder);
        }

        public static IBankIdAuthenticationBuilder UseClientCertificateFromManagedIdentityAzureKeyVaultOptions(this IBankIdAuthenticationBuilder builder)
        {
            builder.UseClientCertificate(() =>
            {
                var options = builder.AuthenticationBuilder.Services.BuildServiceProvider().GetService<IOptions<ClientCertificateFromManagedIdentityAzureKeyVaultOptions>>();
                var client = new AzureKeyVaultManagedIdentityClient(options.Value.AzureKeyVaultUri);
                return client.GetX509Certificate2Async(options.Value.CertificateIdentifier).ConfigureAwait(false).GetAwaiter().GetResult();
            });

            return builder;
        }
    }
}
