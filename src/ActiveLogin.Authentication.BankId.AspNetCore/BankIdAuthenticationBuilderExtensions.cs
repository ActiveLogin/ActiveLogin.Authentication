using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using ActiveLogin.Authentication.BankId.Api.UserMessage;
using ActiveLogin.Authentication.BankId.AspNetCore.Cryptography;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Persistence;
using ActiveLogin.Authentication.BankId.AspNetCore.Resources;
using ActiveLogin.Authentication.Common.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public static class BankIdAuthenticationBuilderExtensions
    {
        internal static IBankIdAuthenticationBuilder AddDefaultServices(this IBankIdAuthenticationBuilder builder)
        {
            var services = builder.AuthenticationBuilder.Services;

            services.TryAddSingleton<IJsonSerializer, SystemRuntimeJsonSerializer>();

            services.TryAddSingleton<IBankIdOrderRefProtector, BankIdOrderRefProtector>();
            services.TryAddSingleton<IBankIdLoginOptionsProtector, BankIdLoginOptionsProtector>();
            services.TryAddSingleton<IBankIdLoginResultProtector, BankIdLoginResultProtector>();

            services.TryAddSingleton<IBankIdUserMessage, BankIdRecommendedUserMessage>();

            services.TryAddTransient<IBankIdResultStore, BankIdResultTraceLoggerStore>();
            services.TryAddTransient<IBankIdUserMessageLocalizer, BankIdUserMessageStringLocalizer>();

            services.AddLocalization(options =>
            {
                options.ResourcesPath = BankIdAuthenticationDefaults.ResourcesPath;
            });

            return builder;
        }

        public static IBankIdAuthenticationBuilder UseBankIdClientCertificate(this IBankIdAuthenticationBuilder builder, Func<X509Certificate2> configureClientCertificate)
        {
            builder.ConfigureBankIdHttpClientHandler(httpClientHandler =>
            {
                var clientCertificate = configureClientCertificate();
                httpClientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
                httpClientHandler.ClientCertificates.Add(clientCertificate);
            });

            return builder;
        }

        public static IBankIdAuthenticationBuilder UseBankIdRootCaCertificate(this IBankIdAuthenticationBuilder builder, Func<X509Certificate2> configureRootCaCertificate)
        {
            builder.ConfigureBankIdHttpClientHandler(httpClientHandler =>
            {
                var rootCaCertificate = configureRootCaCertificate();
                var validator = new X509CertificateChainValidator(rootCaCertificate);
                httpClientHandler.ServerCertificateCustomValidationCallback = validator.Validate;
            });

            return builder;
        }

        public static IBankIdAuthenticationBuilder UseBankIdRootCaCertificate(this IBankIdAuthenticationBuilder builder, string certificateFilePath)
        {
            builder.UseBankIdRootCaCertificate(() => new X509Certificate2(certificateFilePath));

            return builder;
        }
    }
}