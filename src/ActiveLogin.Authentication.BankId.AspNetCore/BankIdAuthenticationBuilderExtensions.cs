using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using ActiveLogin.Authentication.BankId.AspNetCore.Cryptography;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public static class BankIdAuthenticationBuilderExtensions
    {
        public static BankIdAuthenticationBuilder AddBankIdClientCertificate(this BankIdAuthenticationBuilder builder, Func<X509Certificate2> configureClientCertificate)
        {
            builder.ConfigureBankIdHttpClientHandler(httpClientHandler =>
            {
                var clientCertificate = configureClientCertificate();
                httpClientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
                httpClientHandler.ClientCertificates.Add(clientCertificate);
            });

            return builder;
        }

        public static BankIdAuthenticationBuilder AddBankIdRootCaCertificate(this BankIdAuthenticationBuilder builder, Func<X509Certificate2> configureRootCaCertificate)
        {
            builder.ConfigureBankIdHttpClientHandler(httpClientHandler =>
            {
                var rootCaCertificate = configureRootCaCertificate();
                var validator = new X509CertificateChainValidator(rootCaCertificate);
                httpClientHandler.ServerCertificateCustomValidationCallback = validator.Validate;
            });

            return builder;
        }

        public static BankIdAuthenticationBuilder AddBankIdEnvironmentConfiguration(this BankIdAuthenticationBuilder builder, Action<BankIdEnvironmentConfiguration> configureBankIdEnvironment)
        {
            var configuration = new BankIdEnvironmentConfiguration();
            configureBankIdEnvironment(configuration);

            builder.ConfigureBankIdHttpClient(httpClient =>
            {
                httpClient.BaseAddress = configuration.ApiBaseUrl;
            });

            return builder;
        }
    }
}