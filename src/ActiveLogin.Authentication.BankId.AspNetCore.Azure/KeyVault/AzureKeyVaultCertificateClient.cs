using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using static Microsoft.Azure.KeyVault.KeyVaultClient;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Azure.KeyVault
{
    internal class AzureKeyVaultCertificateClient : IDisposable
    {
        public static AzureKeyVaultCertificateClient Create(ClientCertificateFromAzureKeyVaultOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.AzureKeyVaultIdentifier))
                throw new ArgumentException("AzureKeyVaultIdentifier is required");

            if (!options.UseManagedIdentity)
            {
                if (string.IsNullOrWhiteSpace(options.AzureAdClientId))
                    throw new ArgumentException("AzureAdClientId is required when not using ManagedIdentity");
                if (string.IsNullOrWhiteSpace(options.AzureAdClientSecret))
                    throw new ArgumentException("AzureAdClientSecret is required when not using ManagedIdentity");
            }

            KeyVaultClient client;
            if (options.UseManagedIdentity)
            {
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                client = new KeyVaultClient(new AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            }
            else
            {
                var clientCredential = new ClientCredential(options.AzureAdClientId, options.AzureAdClientSecret);
                var callback = GetCallback(clientCredential);
                client = new KeyVaultClient(callback);
            }

            return new AzureKeyVaultCertificateClient(client);
        }

        private static AuthenticationCallback GetCallback(ClientCredential credentials)
        {
            return async (string authority, string resource, string scope) =>
            {
                var authContext = new AuthenticationContext(authority);
                var result = await authContext.AcquireTokenAsync(resource, credentials).ConfigureAwait(false);

                if (result == null)
                {
                    throw new InvalidOperationException("Failed to obtain JWT token");
                }

                return result.AccessToken;
            };
        }

        private const string CertificateContentType = "application/x-pkcs12";
        private readonly KeyVaultClient _keyVaultClient;

        private AzureKeyVaultCertificateClient(KeyVaultClient client)
        {
            _keyVaultClient = client;
        }

        public async Task<X509Certificate2> GetX509Certificate2Async(string keyVaultIdentifier)
        {
            if (IsSecretIdentifier(keyVaultIdentifier))
            {
                return await CertificateFromKeyVaultSecret(keyVaultIdentifier);
            }

            if (IsCertificateIdentifier(keyVaultIdentifier))
            {
                return await CertificateFromKeyVaultCertificate(keyVaultIdentifier);
            }

            throw new ArgumentException($"The identifier did not match secrets or certificates.");
        }

        private async Task<X509Certificate2> CertificateFromKeyVaultSecret(string keyVaultSecretIdentifier)
        {
            var secret = await _keyVaultClient.GetSecretAsync(keyVaultSecretIdentifier).ConfigureAwait(false);
            if (secret.ContentType != CertificateContentType)
            {
                throw new ArgumentException($"This certificate must be of type {CertificateContentType}");
            }

            var certificateBytes = Convert.FromBase64String(secret.Value);

            return GetX509Certificate2(certificateBytes);
        }

        private async Task<X509Certificate2> CertificateFromKeyVaultCertificate(string keyVaultCertificateIdentifier)
        {
            var bundle = await _keyVaultClient.GetCertificateAsync(keyVaultCertificateIdentifier).ConfigureAwait(false);
            var certificateBytes = bundle.Cer;

            return GetX509Certificate2(certificateBytes);
        }

        private bool IsSecretIdentifier(string identifier)
        {
            return identifier.Contains("/secrets/");
        }

        private bool IsCertificateIdentifier(string identifier)
        {
            return identifier.Contains("/certificates/");
        }

        private X509Certificate2 GetX509Certificate2(byte[] certificate)
        {
            var exportedCertCollection = new X509Certificate2Collection();
            exportedCertCollection.Import(certificate, string.Empty, X509KeyStorageFlags.MachineKeySet);

            return exportedCertCollection.Cast<X509Certificate2>().First(x => x.HasPrivateKey);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _keyVaultClient?.Dispose();
            }
        }

        ~AzureKeyVaultCertificateClient()
        {
            Dispose(false);
        }
    }
}
