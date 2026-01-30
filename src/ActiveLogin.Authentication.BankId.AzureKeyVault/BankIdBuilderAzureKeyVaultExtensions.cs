using System.Security.Cryptography.X509Certificates;

using ActiveLogin.Authentication.BankId.Core;

using Microsoft.Extensions.Configuration;

namespace ActiveLogin.Authentication.BankId.AzureKeyVault;
public static class BankIdBuilderAzureKeyVaultExtensions
{
    /// <summary>
    /// Use client certificate for authenticating against the BankID API from Azure Key Vault.
    /// </summary>
    /// <param name="builder">The BankID builder.</param>
    /// <param name="configurationSection">Configuration section to bind the Key Vault options from.</param>
    /// <param name="keyStorageFlags">
    /// Specifies how the private key of the certificate is stored and managed when loaded.
    /// Defaults to <see cref="X509KeyStorageFlags.DefaultKeySet"/>.
    /// </param>
    /// <returns></returns>
    public static IBankIdBuilder UseClientCertificateFromAzureKeyVault(this IBankIdBuilder builder, IConfigurationSection configurationSection, X509KeyStorageFlags keyStorageFlags = X509KeyStorageFlags.DefaultKeySet)
    {
        var options = new ClientCertificateFromAzureKeyVaultOptions();
        configurationSection.Bind(options);
        return UseClientCertificateFromAzureKeyVault(builder, options, keyStorageFlags);
    }

    /// <summary>
    /// Use client certificate for authenticating against the BankID API from Azure KeyVault.
    /// </summary>
    /// <param name="builder">The BankID builder.</param>
    /// <param name="configureOptions">Callback to configure the Key Vault options.</param>
    /// <param name="keyStorageFlags">
    /// Specifies how the private key of the certificate is stored and managed when loaded.
    /// Defaults to <see cref="X509KeyStorageFlags.DefaultKeySet"/>.
    /// </param>
    /// <returns>The updated <see cref="IBankIdBuilder"/>.</returns>
    public static IBankIdBuilder UseClientCertificateFromAzureKeyVault(this IBankIdBuilder builder, Action<ClientCertificateFromAzureKeyVaultOptions> configureOptions, X509KeyStorageFlags keyStorageFlags = X509KeyStorageFlags.DefaultKeySet)
    {
        var options = new ClientCertificateFromAzureKeyVaultOptions();
        configureOptions(options);
        return UseClientCertificateFromAzureKeyVault(builder, options, keyStorageFlags);
    }

    /// <summary>
    /// Use client certificate for authenticating against the BankID API from Azure Key Vault.
    /// </summary>
    /// <param name="builder">The BankID builder.</param>
    /// <param name="options">The Key Vault options.</param>
    /// <param name="keyStorageFlags">
    /// Specifies how the private key of the certificate is stored and managed when loaded.
    /// Defaults to <see cref="X509KeyStorageFlags.DefaultKeySet"/>.
    /// </param>
    /// <returns>The updated <see cref="IBankIdBuilder"/>.</returns>
    public static IBankIdBuilder UseClientCertificateFromAzureKeyVault(this IBankIdBuilder builder, ClientCertificateFromAzureKeyVaultOptions options, X509KeyStorageFlags keyStorageFlags = X509KeyStorageFlags.DefaultKeySet)
    {
        if (string.IsNullOrWhiteSpace(options.AzureKeyVaultSecretName))
        {
            throw new ArgumentException("AzureKeyVaultSecretName is required");
        }

        builder.UseClientCertificate(() =>
        {
            var keyVaultCertificateClient = AzureKeyVaultCertificateClient.Create(options);

            return keyVaultCertificateClient.GetX509Certificate2(options.AzureKeyVaultSecretName, keyStorageFlags);
        });

        return builder;
    }


    /// <summary>
    /// Add client certificate for authenticating against the BankID API from Azure Key Vault.
    /// </summary>
    /// <param name="builder">The BankID builder.</param>
    /// <param name="configurationSection">Configuration section to bind the Key Vault options from.</param>
    /// <param name="keyStorageFlags">
    /// Specifies how the private key of the certificate is stored and managed when loaded.
    /// Defaults to <see cref="X509KeyStorageFlags.DefaultKeySet"/>.
    /// </param>
    /// <returns>The updated <see cref="IBankIdBuilder"/>.</returns>
    public static IBankIdBuilder AddClientCertificateFromAzureKeyVault(this IBankIdBuilder builder, IConfigurationSection configurationSection, X509KeyStorageFlags keyStorageFlags = X509KeyStorageFlags.DefaultKeySet)
    {
        var options = new ClientCertificateFromAzureKeyVaultOptions();
        configurationSection.Bind(options);
        return AddClientCertificateFromAzureKeyVault(builder, options, keyStorageFlags);
    }

    /// <summary>
    /// Add client certificate for authenticating against the BankID API from Azure KeyVault.
    /// </summary>
    /// <param name="builder">The BankID builder.</param>
    /// <param name="configureOptions">Callback to configure the Key Vault options.</param>
    /// <param name="keyStorageFlags">
    /// Specifies how the private key of the certificate is stored and managed when loaded.
    /// Defaults to <see cref="X509KeyStorageFlags.DefaultKeySet"/>.
    /// </param>
    /// <returns>The updated <see cref="IBankIdBuilder"/>.</returns>
    public static IBankIdBuilder AddClientCertificateFromAzureKeyVault(this IBankIdBuilder builder, Action<ClientCertificateFromAzureKeyVaultOptions> configureOptions, X509KeyStorageFlags keyStorageFlags = X509KeyStorageFlags.DefaultKeySet)
    {
        var options = new ClientCertificateFromAzureKeyVaultOptions();
        configureOptions(options);
        return AddClientCertificateFromAzureKeyVault(builder, options, keyStorageFlags);
    }

    /// <summary>
    /// Add client certificate for authenticating against the BankID API from Azure Key Vault.
    /// </summary>
    /// <param name="builder">The BankID builder.</param>
    /// <param name="options">The Azure Key Vault configuration options.</param>
    /// <param name="keyStorageFlags">
    /// Specifies how the private key of the certificate is stored and managed when loaded.
    /// Defaults to <see cref="X509KeyStorageFlags.DefaultKeySet"/>.
    /// </param>
    /// <returns>The updated <see cref="IBankIdBuilder"/>.</returns>
    public static IBankIdBuilder AddClientCertificateFromAzureKeyVault(this IBankIdBuilder builder, ClientCertificateFromAzureKeyVaultOptions options, X509KeyStorageFlags keyStorageFlags = X509KeyStorageFlags.DefaultKeySet)
    {
        if (string.IsNullOrWhiteSpace(options.AzureKeyVaultSecretName))
        {
            throw new ArgumentException("AzureKeyVaultSecretName is required");
        }

        builder.AddClientCertificate(() =>
        {
            var keyVaultCertificateClient = AzureKeyVaultCertificateClient.Create(options);

            return keyVaultCertificateClient.GetX509Certificate2(options.AzureKeyVaultSecretName, keyStorageFlags);
        });

        return builder;
    }
}
