using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using ActiveLogin.Authentication.GrandId.Api;
using ActiveLogin.Authentication.GrandId.AspNetCore.Cryptography;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public static class GrandIdAuthenticationBuilderExtensions
    {
        public static GrandIdAuthenticationBuilder AddGrandIdClientCertificate(this GrandIdAuthenticationBuilder builder, Func<X509Certificate2> configureClientCertificate)
        {
            builder.ConfigureGrandIdHttpClientHandler(httpClientHandler =>
            {
                var clientCertificate = configureClientCertificate();
                httpClientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
                httpClientHandler.ClientCertificates.Add(clientCertificate);
            });

            return builder;
        }

        public static GrandIdAuthenticationBuilder AddGrandIdRootCaCertificate(this GrandIdAuthenticationBuilder builder, Func<X509Certificate2> configureRootCaCertificate)
        {
            builder.ConfigureGrandIdHttpClientHandler(httpClientHandler =>
            {
                var rootCaCertificate = configureRootCaCertificate();
                var validator = new X509CertificateChainValidator(rootCaCertificate);
                httpClientHandler.ServerCertificateCustomValidationCallback = validator.Validate;
            });

            return builder;
        }

        public static GrandIdAuthenticationBuilder AddGrandIdRootCaCertificate(this GrandIdAuthenticationBuilder builder, string certificateFilePath)
        {
            builder.AddGrandIdRootCaCertificate(() => new X509Certificate2(certificateFilePath));

            return builder;
        }

        public static GrandIdAuthenticationBuilder AddGrandIdEnvironmentConfiguration(this GrandIdAuthenticationBuilder builder, Action<GrandIdEnvironmentConfiguration> configureGrandIdEnvironment)
        {
            var configuration = new GrandIdEnvironmentConfiguration();
            configureGrandIdEnvironment(configuration);

            builder.ConfigureGrandIdHttpClient(httpClient =>
            {
                httpClient.BaseAddress = configuration.ApiBaseUrl;
            });

            return builder;
        }

        public static GrandIdAuthenticationBuilder AddGrandIdTestEnvironment(this GrandIdAuthenticationBuilder builder)
        {
            builder.AddGrandIdEnvironmentConfiguration(configuration =>
            {
                configuration.ApiBaseUrl = GrandIdUrls.TestApiBaseUrl;
            });

            return builder;
        }

        public static GrandIdAuthenticationBuilder AddGrandIdProdEnvironment(this GrandIdAuthenticationBuilder builder)
        {
            builder.AddGrandIdEnvironmentConfiguration(configuration =>
            {
                configuration.ApiBaseUrl = GrandIdUrls.ProdApiBaseUrl;
            });

            return builder;
        }
    }
}