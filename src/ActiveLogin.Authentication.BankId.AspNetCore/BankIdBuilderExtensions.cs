using System;
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;

using ActiveLogin.Authentication.BankId.Api.UserMessage;
using ActiveLogin.Authentication.BankId.AspNetCore;
using ActiveLogin.Authentication.BankId.AspNetCore.ClaimsTransformation;
using ActiveLogin.Authentication.BankId.AspNetCore.Cryptography;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.EndUserContext;
using ActiveLogin.Authentication.BankId.AspNetCore.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.AspNetCore.Persistence;
using ActiveLogin.Authentication.BankId.AspNetCore.Qr;
using ActiveLogin.Authentication.BankId.AspNetCore.StateHandling;
using ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice;
using ActiveLogin.Authentication.BankId.AspNetCore.UserMessage;

using Microsoft.AspNetCore.Http;
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
            services.TryAddTransient<IBankIdQrStartStateProtector, BankIdQrStartStateProtector>();
            services.TryAddTransient<IBankIdLoginOptionsProtector, BankIdLoginOptionsProtector>();
            services.TryAddTransient<IBankIdLoginResultProtector, BankIdLoginResultProtector>();

            services.TryAddTransient<IBankIdInvalidStateHandler, BankIdCancelUrlInvalidStateHandler>();

            services.TryAddTransient<IBankIdUserMessage, BankIdRecommendedUserMessage>();
            services.TryAddTransient<IBankIdSupportedDeviceDetector, BankIdSupportedDeviceDetector>();

            services.TryAddTransient<IBankIdUserMessageLocalizer, BankIdUserMessageStringLocalizer>();

            services.TryAddTransient<IBankIdQrCodeContentGenerator, BankIdQrCodeContentGenerator>();
            services.TryAddTransient<IBankIdQrCodeGenerator, BankIdMissingQrCodeGenerator>();

            services.TryAddTransient<IBankIdEventTrigger, BankIdEventTrigger>();

            builder.UseAuthRequestUserDataResolver<BankIdAuthRequestEmptyUserDataResolver>();
            builder.UseEndUserIpResolver<BankIdRemoteIpAddressEndUserIpResolver>();

            builder.AddClaimsTransformer<BankIdDefaultClaimsTransformer>();

            builder.AddEventListener<BankIdLoggerEventListener>();
            builder.AddEventListener<BankIdResultStoreEventListener>();

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

        /// <summary>
        /// Use client certificate for authenticating against the BankID API.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureClientCertificate">The certificate to use.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Use this root certificate for verifying the certificate of BankID API.
        /// Use only if the root certificate can't be installed on the machine.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureRootCaCertificate">The root certificate provided by BankID (*.crt)</param>
        /// <returns></returns>
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

        /// <summary>
        /// Use this root certificate for verifying the certificate of BankID API.
        /// Use only if the root certificate can't be installed on the machine.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="certificateFilePath">The file path to the root certificate provided by BankID (*.crt)</param>
        /// <returns></returns>
        public static IBankIdBuilder UseRootCaCertificate(this IBankIdBuilder builder, string certificateFilePath)
        {
            builder.UseRootCaCertificate(() => new X509Certificate2(certificateFilePath));

            return builder;
        }

        /// <summary>
        /// Set the class that will be used to resolve end user ip.
        /// </summary>
        /// <typeparam name="TEndUserIpResolverImplementation"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IBankIdBuilder UseEndUserIpResolver<TEndUserIpResolverImplementation>(this IBankIdBuilder builder) where TEndUserIpResolverImplementation : class, IBankIdEndUserIpResolver
        {
            builder.AuthenticationBuilder.Services.AddTransient<IBankIdEndUserIpResolver, TEndUserIpResolverImplementation>();

            return builder;
        }

        /// <summary>
        /// Set resolver that will be used to resolve the ip of the end user.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="resolver"></param>
        /// <returns></returns>
        public static IBankIdBuilder UseEndUserIpResolver(this IBankIdBuilder builder, Func<HttpContext, string> resolver)
        {
            builder.AuthenticationBuilder.Services.AddTransient<IBankIdEndUserIpResolver>(x => new BankIdDynamicEndUserIpResolver(resolver));

            return builder;
        }

        /// <summary>
        /// Set what user data to supply to the auth request.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="authUserData"></param>
        /// <returns></returns>
        public static IBankIdBuilder UseAuthRequestUserData(this IBankIdBuilder builder, BankIdAuthUserData authUserData)
        {
            builder.AuthenticationBuilder.Services.AddTransient<IBankIdAuthRequestUserDataResolver>(x => new BankIdAuthRequestStaticUserDataResolver(authUserData));

            return builder;
        }

        /// <summary>
        /// Set what user data to supply to the auth request.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="authUserData"></param>
        /// <returns></returns>
        public static IBankIdBuilder UseAuthRequestUserData(this IBankIdBuilder builder, Action<BankIdAuthUserData> authUserData)
        {
            var authUserDataResult = new BankIdAuthUserData();
            authUserData(authUserDataResult);
            UseAuthRequestUserData(builder, authUserDataResult);

            return builder;
        }

        /// <summary>
        /// Add a custom event listener.
        /// </summary>
        /// <typeparam name="TBankIdEventListenerImplementation"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IBankIdBuilder AddEventListener<TBankIdEventListenerImplementation>(this IBankIdBuilder builder) where TBankIdEventListenerImplementation : class, IBankIdEventListener
        {
            builder.AuthenticationBuilder.Services.AddTransient<IBankIdEventListener, TBankIdEventListenerImplementation>();

            return builder;
        }

        /// <summary>
        /// Add an event listener that will serialize and write all events to debug.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IBankIdBuilder AddDebugEventListener(this IBankIdBuilder builder)
        {
            builder.AddEventListener<BankIdDebugEventListener>();

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
            builder.AuthenticationBuilder.Services.AddTransient<IBankIdResultStore, TResultStoreImplementation>();

            return builder;
        }

        /// <summary>
        /// Use a custom user data resolver.
        /// </summary>
        /// <typeparam name="TBankIdAuthRequestUserDataResolverImplementation"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IBankIdBuilder UseAuthRequestUserDataResolver<TBankIdAuthRequestUserDataResolverImplementation>(this IBankIdBuilder builder) where TBankIdAuthRequestUserDataResolverImplementation : class, IBankIdAuthRequestUserDataResolver
        {
            builder.AuthenticationBuilder.Services.AddTransient<IBankIdAuthRequestUserDataResolver, TBankIdAuthRequestUserDataResolverImplementation>();

            return builder;
        }

        /// <summary>
        /// Add a custom claims transaformer.
        /// </summary>
        /// <typeparam name="TBankIdClaimsTransformerImplementation"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IBankIdBuilder AddClaimsTransformer<TBankIdClaimsTransformerImplementation>(this IBankIdBuilder builder) where TBankIdClaimsTransformerImplementation : class, IBankIdClaimsTransformer
        {
            builder.AuthenticationBuilder.Services.AddTransient<IBankIdClaimsTransformer, TBankIdClaimsTransformerImplementation>();

            return builder;
        }
    }
}
