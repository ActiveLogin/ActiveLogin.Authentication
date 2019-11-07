using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using ActiveLogin.Authentication.BankId.Api.UserMessage;
using ActiveLogin.Authentication.BankId.AspNetCore.Cryptography;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Persistence;
using ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice;
using ActiveLogin.Authentication.BankId.AspNetCore.UserMessage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public static class BankIdAuthenticationBuilderExtensions
    {
        internal static IBankIdAuthenticationBuilder AddDefaultServices(this IBankIdAuthenticationBuilder builder)
        {
            var services = builder.AuthenticationBuilder.Services;

            services.TryAddTransient<IBankIdOrderRefProtector, BankIdOrderRefProtector>();
            services.TryAddTransient<IBankIdLoginOptionsProtector, BankIdLoginOptionsProtector>();
            services.TryAddTransient<IBankIdLoginResultProtector, BankIdLoginResultProtector>();

            services.TryAddTransient<IBankIdUserMessage, BankIdRecommendedUserMessage>();
            services.TryAddTransient<IBankIdSupportedDeviceDetector, BankIdSupportedDeviceDetector>();

            services.TryAddTransient<IBankIdResultStore, BankIdResultTraceLoggerStore>();
            services.TryAddTransient<IBankIdUserMessageLocalizer, BankIdUserMessageStringLocalizer>();

            services.AddLocalization(options =>
            {
                options.ResourcesPath = BankIdAuthenticationDefaults.ResourcesPath;
            });

            return builder;
        }

        public static IBankIdAuthenticationBuilder UseClientCertificate(this IBankIdAuthenticationBuilder builder, Func<X509Certificate2> configureClientCertificate)
        {
            builder.ConfigureHttpClientHandler(httpClientHandler =>
            {
                var clientCertificate = configureClientCertificate();
                httpClientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
                httpClientHandler.ClientCertificates.Add(clientCertificate);
            });

            return builder;
        }

        public static IBankIdAuthenticationBuilder UseRootCaCertificate(this IBankIdAuthenticationBuilder builder, Func<X509Certificate2> configureRootCaCertificate)
        {
            builder.ConfigureHttpClientHandler(httpClientHandler =>
            {
                var rootCaCertificate = configureRootCaCertificate();
                var validator = new X509CertificateChainValidator(rootCaCertificate);
                httpClientHandler.ServerCertificateCustomValidationCallback = validator.Validate;
            });

            return builder;
        }

        public static IBankIdAuthenticationBuilder UseRootCaCertificate(this IBankIdAuthenticationBuilder builder, string certificateFilePath)
        {
            builder.UseRootCaCertificate(() => new X509Certificate2(certificateFilePath));

            return builder;
        }
    }
}
