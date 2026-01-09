using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace ActiveLogin.Authentication.BankId.AzureKeyVault;

internal class AzureKeyVaultCertificateClient(SecretClient secretClient)
{
    public static AzureKeyVaultCertificateClient Create(ClientCertificateFromAzureKeyVaultOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.AzureKeyVaultUri))
        {
            throw new ArgumentException("AzureKeyVaultUri is required");
        }

        var tokenCredentials = GetTokenCredential(options);
        var secretClient = new SecretClient(new Uri(options.AzureKeyVaultUri), tokenCredentials);

        return new AzureKeyVaultCertificateClient(secretClient);
    }

    private static TokenCredential GetTokenCredential(ClientCertificateFromAzureKeyVaultOptions options)
    {
        if (!string.IsNullOrEmpty(options.AzureAdTenantId) &&
            !string.IsNullOrEmpty(options.AzureAdClientId) &&
            !string.IsNullOrEmpty(options.AzureAdClientSecret))
        {
            return new ClientSecretCredential(
                options.AzureAdTenantId,
                options.AzureAdClientId,
                options.AzureAdClientSecret
            );
        }

        if (!string.IsNullOrEmpty(options.AzureManagedIdentityClientId))
        {
            return new DefaultAzureCredential(new DefaultAzureCredentialOptions()
            {
                ManagedIdentityClientId = options.AzureManagedIdentityClientId
            });
        }

        return new DefaultAzureCredential();
    }

    private const string CertificateContentType = "application/x-pkcs12";

    public X509Certificate2 GetX509Certificate2(string keyVaultSecretKey)
    {
        var secret = secretClient.GetSecret(keyVaultSecretKey).Value;
        if (secret.Properties.ContentType != CertificateContentType)
        {
            throw new ArgumentException($"This certificate must be of type {CertificateContentType}");
        }

        var certificateBytes = Convert.FromBase64String(secret.Value);

        return GetX509Certificate2(certificateBytes);
    }

    private static X509Certificate2 GetX509Certificate2(byte[] certificate)
    {
        var flags = GetPlatformKeyStorageFlags();

        var certs = X509CertificateLoader.LoadPkcs12Collection(certificate, password: null, flags);

        return certs.First(c => c.HasPrivateKey);
    }

    private static X509KeyStorageFlags GetPlatformKeyStorageFlags()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable;
        }

        // Linux / Azure App Service / Containers
        return X509KeyStorageFlags.EphemeralKeySet;
    }
}
