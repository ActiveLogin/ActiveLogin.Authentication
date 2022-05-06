using ActiveLogin.Authentication.BankId.AzureKeyVault;
using ActiveLogin.Authentication.BankId.Core;

using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;
public static class BankIdBuilderAzureKeyVaultExtensions
{
    /// <summary>
    /// Use client certificate for authenticating against the BankID API from Azure Key Vault.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configurationSection">Configuration section to bind the Key Vault options from.</param>
    /// <returns></returns>
    public static IBankIdBuilder UseClientCertificateFromAzureKeyVault(this IBankIdBuilder builder, IConfigurationSection configurationSection)
    {
        var options = new ClientCertificateFromAzureKeyVaultOptions();
        configurationSection.Bind(options);
        return UseClientCertificateFromAzureKeyVault(builder, options);
    }

    /// <summary>
    /// Use client certificate for authenticating against the BankID API from Azure KeyVault.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="configureOptions">Callback to configure the Key Vault options.</param>
    /// <returns></returns>
    public static IBankIdBuilder UseClientCertificateFromAzureKeyVault(this IBankIdBuilder builder, Action<ClientCertificateFromAzureKeyVaultOptions> configureOptions)
    {
        var options = new ClientCertificateFromAzureKeyVaultOptions();
        configureOptions(options);
        return UseClientCertificateFromAzureKeyVault(builder, options);
    }

    /// <summary>
    /// Use client certificate for authenticating against the BankID API from Azure Key Vault.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="options">The Key Vault options.</param>
    /// <returns></returns>
    public static IBankIdBuilder UseClientCertificateFromAzureKeyVault(this IBankIdBuilder builder, ClientCertificateFromAzureKeyVaultOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.AzureKeyVaultSecretName))
        {
            throw new ArgumentException("AzureKeyVaultSecretName is required");
        }

        builder.UseClientCertificate(() =>
        {
            var keyVaultCertificateClient = AzureKeyVaultCertificateClient.Create(options);

            return keyVaultCertificateClient.GetX509Certificate2(options.AzureKeyVaultSecretName);
        });

        return builder;
    }
}
