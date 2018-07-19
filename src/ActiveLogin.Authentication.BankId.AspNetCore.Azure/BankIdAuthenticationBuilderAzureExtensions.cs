using System;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Azure
{
    public static class BankIdAuthenticationBuilderAzureExtensions
    {
        public static BankIdAuthenticationBuilder AddBankIdClientCertificateFromAzureKeyVault(this BankIdAuthenticationBuilder builder, ClientCertificateFromAzureKeyVaultOptions options)
        {
            builder.AddBankIdClientCertificate(() =>
            {
                using (var keyVaultCertificateClient = new AzureKeyVaultCertificateClient(options.AzureAdClientId, options.AzureAdClientSecret))
                {
                    return keyVaultCertificateClient.GetX509Certificate2Async(options.KeyVaultBaseUrl, options.KeyVaultSecretName).GetAwaiter().GetResult();
                }
            });

            return builder;
        }
    }
}
