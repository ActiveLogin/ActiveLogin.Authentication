using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Azure
{
    internal class AzureKeyVaultCertificateClient
    {
        public static AzureKeyVaultCertificateClient Create(ClientCertificateFromAzureKeyVaultOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.AzureKeyVaultUri))
            {
                throw new ArgumentException("AzureKeyVaultUri is required");
            }

            if (!options.UseManagedIdentity)
            {
                if (string.IsNullOrWhiteSpace(options.AzureAdTenantId))
                {
                    throw new ArgumentException("AzureAdTenantId is required when not using ManagedIdentity");
                }

                if (string.IsNullOrWhiteSpace(options.AzureAdClientId))
                {
                    throw new ArgumentException("AzureAdClientId is required when not using ManagedIdentity");
                }

                if (string.IsNullOrWhiteSpace(options.AzureAdClientSecret))
                {
                    throw new ArgumentException("AzureAdClientSecret is required when not using ManagedIdentity");
                }
            }

            var tokenCredentials = GetTokenCredential(options);
            var secretClient = new SecretClient(new Uri(options.AzureKeyVaultUri), tokenCredentials);

            return new AzureKeyVaultCertificateClient(secretClient);
        }

        private static TokenCredential GetTokenCredential(ClientCertificateFromAzureKeyVaultOptions options)
        {
            if (!options.UseManagedIdentity)
            {
                return new ClientSecretCredential(
                    options.AzureAdTenantId,
                    options.AzureAdClientId,
                    options.AzureAdClientSecret
                );
            }

            return new ManagedIdentityCredential();
        }

        private const string CertificateContentType = "application/x-pkcs12";
        private readonly SecretClient _secretClient;

        private AzureKeyVaultCertificateClient(SecretClient secretClient)
        {
            _secretClient = secretClient;
        }

        public X509Certificate2 GetX509Certificate2(string keyVaultSecretKey)
        {
            var secret = _secretClient.GetSecret(keyVaultSecretKey).Value;
            if (secret.Properties.ContentType != CertificateContentType)
            {
                throw new ArgumentException($"This certificate must be of type {CertificateContentType}");
            }

            var certificateBytes = Convert.FromBase64String(secret.Value);

            return GetX509Certificate2(certificateBytes);
        }

        private X509Certificate2 GetX509Certificate2(byte[] certificate)
        {
            var exportedCertCollection = new X509Certificate2Collection();
            exportedCertCollection.Import(certificate, string.Empty, X509KeyStorageFlags.MachineKeySet);

            return exportedCertCollection.Cast<X509Certificate2>().First(x => x.HasPrivateKey);
        }
    }
}
