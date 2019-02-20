using System;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using static Microsoft.Azure.KeyVault.KeyVaultClient;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Azure.KeyVault
{
    public class KeyVaultClientFactory
    {
        public static KeyVaultClient Create(ClientCertificateFromAzureKeyVaultOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.AzureKeyVaultSecretIdentifier))
                throw new ArgumentException("AzureKeyVaultSecretIdentifier is required");

            if (!options.UseManagedIdentity)
            {
                if (string.IsNullOrWhiteSpace(options.AzureAdClientId))
                    throw new ArgumentException("AzureAdClientId is required when not using ManagedIdentity");
                if (string.IsNullOrWhiteSpace(options.AzureAdClientSecret))
                    throw new ArgumentException("AzureAdClientSecret is required when not using ManagedIdentity");
            }

            if (options.UseManagedIdentity)
            {
                var azureServiceTokenProvider = new AzureServiceTokenProvider();
                return new KeyVaultClient(new AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            }
            else
            {
                var clientCredential = new ClientCredential(options.AzureAdClientId, options.AzureAdClientSecret);
                var callback = GetCallback(clientCredential);
                return new KeyVaultClient(callback);
            }
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
    }
}
