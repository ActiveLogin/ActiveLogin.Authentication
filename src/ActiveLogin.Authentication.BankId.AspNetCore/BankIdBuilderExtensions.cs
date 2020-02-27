using System;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using ActiveLogin.Authentication.BankId.Api.UserMessage;
using ActiveLogin.Authentication.BankId.AspNetCore;
using ActiveLogin.Authentication.BankId.AspNetCore.Cryptography;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Persistence;
using ActiveLogin.Authentication.BankId.AspNetCore.Qr;
using ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice;
using ActiveLogin.Authentication.BankId.AspNetCore.UserMessage;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BankIdBuilderExtensions
    {
        internal static IBankIdBuilder AddDefaultServices(this IBankIdBuilder builder)
        {
            var services = builder.AuthenticationBuilder.Services;

            services.AddControllersWithViews();
            services.AddHttpContextAccessor();

            services.AddLocalization(options =>
            {
                options.ResourcesPath = BankIdDefaults.ResourcesPath;
            });

            services.TryAddTransient<IBankIdOrderRefProtector, BankIdOrderRefProtector>();
            services.TryAddTransient<IBankIdLoginOptionsProtector, BankIdLoginOptionsProtector>();
            services.TryAddTransient<IBankIdLoginResultProtector, BankIdLoginResultProtector>();

            services.TryAddTransient<IBankIdUserMessage, BankIdRecommendedUserMessage>();
            services.TryAddTransient<IBankIdSupportedDeviceDetector, BankIdSupportedDeviceDetector>();

            services.TryAddTransient<IBankIdUserMessageLocalizer, BankIdUserMessageStringLocalizer>();

            services.TryAddTransient<IBankIdQrCodeGenerator, BankIdMissingQrCodeGenerator>();

            builder.AddResultStore<BankIdResultTraceLoggerStore>();

            return builder;
        }

        internal static IBankIdBuilder UseUserAgent(this IBankIdBuilder builder, ProductInfoHeaderValue productInfoHeaderValue)
        {
            builder.ConfigureHttpClient(httpClient =>
            {
                httpClient.DefaultRequestHeaders.UserAgent.Clear();
                httpClient.DefaultRequestHeaders.UserAgent.Add(productInfoHeaderValue);
            });

            return builder;
        }

        public static IBankIdBuilder UseClientCertificate(this IBankIdBuilder builder, Func<X509Certificate2> configureClientCertificate)
        {
            builder.ConfigureHttpClientHandler(httpClientHandler =>
            {
                var clientCertificate = configureClientCertificate();
                httpClientHandler.SslOptions.ClientCertificates ??= new X509Certificate2Collection();
                httpClientHandler.SslOptions.ClientCertificates.Add(clientCertificate);
            });

            return builder;
        }

        public static IBankIdBuilder UseClientCertificateResolver(this IBankIdBuilder builder, Func<ServiceProvider, X509CertificateCollection, string, X509Certificate> configureClientCertificateResolver)
        {
            builder.ConfigureHttpClientHandler(httpClientHandler =>
            {
                var services = builder.AuthenticationBuilder.Services;
                var serviceProvider = services.BuildServiceProvider();

                httpClientHandler.SslOptions.LocalCertificateSelectionCallback =
                    (sender, host, certificates, certificate, issuers) => configureClientCertificateResolver(serviceProvider, certificates, host);
            });

            return builder;
        }

        public static IBankIdBuilder UseRootCaCertificate(this IBankIdBuilder builder, Func<X509Certificate2> configureRootCaCertificate)
        {
            builder.ConfigureHttpClientHandler(httpClientHandler =>
            {
                var rootCaCertificate = configureRootCaCertificate();
                var validator = new X509CertificateChainValidator(rootCaCertificate);
                httpClientHandler.SslOptions.RemoteCertificateValidationCallback = validator.Validate;
            });

            return builder;
        }

        public static IBankIdBuilder UseRootCaCertificate(this IBankIdBuilder builder, string certificateFilePath)
        {
            builder.UseRootCaCertificate(() => new X509Certificate2(certificateFilePath));

            return builder;
        }

        /// <summary>
        /// Adds a class that will be called when BankID returns a valid signed in user.
        /// </summary>
        /// <typeparam name="TResultStoreImplementation"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IBankIdBuilder AddResultStore<TResultStoreImplementation>(this IBankIdBuilder builder) where TResultStoreImplementation : class, IBankIdResultStore
        {
            builder.AuthenticationBuilder.Services.TryAddTransient<IBankIdResultStore, TResultStoreImplementation>();

            return builder;
        }
    }
}
