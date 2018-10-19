﻿using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Azure.KeyVault
{
    internal class AzureKeyVaultCertificateClient : IDisposable
    {
        private const string CertificateContentType = "application/x-pkcs12";

        private readonly ClientCredential _clientCredential;
        private readonly KeyVaultClient _keyVaultClient;

        public AzureKeyVaultCertificateClient(string clientId, string clientSecret)
        {
            _clientCredential = new ClientCredential(clientId, clientSecret);
            _keyVaultClient = new KeyVaultClient(GetToken);
        }

        public async Task<X509Certificate2> GetX509Certificate2Async(string keyVaultSecretIdentifier)
        {
            var secret = await _keyVaultClient.GetSecretAsync(keyVaultSecretIdentifier).ConfigureAwait(false);
            if (secret.ContentType != CertificateContentType)
            {
                throw new ArgumentException($"This certificate must be of type {CertificateContentType}");
            }

            var certificateBytes = Convert.FromBase64String(secret.Value);
            var certificate = GetX509Certificate2(certificateBytes);

            return certificate;
        }

        private X509Certificate2 GetX509Certificate2(byte[] certificate)
        {
            var exportedCertCollection = new X509Certificate2Collection();
            exportedCertCollection.Import(certificate, string.Empty, X509KeyStorageFlags.MachineKeySet);

            return exportedCertCollection.Cast<X509Certificate2>().First(x => x.HasPrivateKey);
        }

        public async Task<string> GetToken(string authority, string resource, string scope)
        {
            var authContext = new AuthenticationContext(authority);
            var result = await authContext.AcquireTokenAsync(resource, _clientCredential).ConfigureAwait(false);

            if (result == null)
            {
                throw new InvalidOperationException("Failed to obtain JWT token");
            }

            return result.AccessToken;
        }

        public void Dispose()
        {
            _keyVaultClient?.Dispose();
        }
    }
}
