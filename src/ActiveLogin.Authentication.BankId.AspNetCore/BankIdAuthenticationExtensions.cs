using System;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public static class BankIdAuthenticationExtensions
    {
        public static AuthenticationBuilder AddBankId(this AuthenticationBuilder builder, X509Certificate2 clientCertificate)
        {
            return AddBankId(
                builder,
                clientCertificate,
                BankIdAuthenticationDefaults.AuthenticationScheme,
                BankIdAuthenticationDefaults.DisplayName,
                options => { },
                handler => { }
            );
        }

        public static AuthenticationBuilder AddBankId(this AuthenticationBuilder builder, X509Certificate2 clientCertificate, string authenticationScheme, Action<BankIdAuthenticationOptions> configureOptions)
        {
            return AddBankId(
                builder,
                clientCertificate,
                authenticationScheme,
                BankIdAuthenticationDefaults.DisplayName,
                configureOptions,
                handler => { }
            );
        }

        public static AuthenticationBuilder AddBankId(this AuthenticationBuilder builder, X509Certificate2 clientCertificate, Action<BankIdAuthenticationOptions> configureOptions)
        {
            return AddBankId(
                builder,
                clientCertificate,
                BankIdAuthenticationDefaults.AuthenticationScheme,
                BankIdAuthenticationDefaults.DisplayName,
                configureOptions,
                handler => { }
            );
        }

        public static AuthenticationBuilder AddBankId(this AuthenticationBuilder builder, X509Certificate2 clientCertificate, string authenticationScheme, string displayName, Action<BankIdAuthenticationOptions> configureOptions)
        {
            return AddBankId(
                builder,
                clientCertificate,
                authenticationScheme,
                displayName,
                configureOptions,
                handler => { }
            );
        }

        public static AuthenticationBuilder AddBankId(this AuthenticationBuilder builder, X509Certificate2 clientCertificate, Action<HttpClientHandler> configureHttpClientHandler)
        {
            return AddBankId(
                builder,
                clientCertificate,
                BankIdAuthenticationDefaults.AuthenticationScheme,
                BankIdAuthenticationDefaults.DisplayName,
                option => { },
                configureHttpClientHandler
            );
        }

        public static AuthenticationBuilder AddBankId(this AuthenticationBuilder builder, X509Certificate2 clientCertificate, string authenticationScheme, string displayName, Action<BankIdAuthenticationOptions> configureOptions, Action<HttpClientHandler> configureHttpClientHandler)
        {
            return AddBankId(
                builder,
                authenticationScheme,
                displayName,
                configureOptions,
                handler =>
                {
                    if (clientCertificate != null)
                    {
                        handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                        handler.ClientCertificates.Add(clientCertificate);
                    }

                    configureHttpClientHandler?.Invoke(handler);
                }
            );
        }

        public static AuthenticationBuilder AddBankId(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<BankIdAuthenticationOptions> configureOptions, Action<HttpClientHandler> configureHttpClientHandler)
        {
            AddBankIdServices(builder.Services, configureHttpClientHandler);

            return builder.AddScheme<BankIdAuthenticationOptions, BankIdAuthenticationHandler>(
                authenticationScheme,
                displayName,
                configureOptions
            );
        }

        private static void AddBankIdServices(IServiceCollection services, Action<HttpClientHandler> configureHttpClientHandler)
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<BankIdAuthenticationOptions>, BankIdAuthenticationPostConfigureOptions>());

            services.TryAddSingleton<IBankIdOrderRefProtector, BankIdOrderRefProtector>();
            services.TryAddSingleton<IBankIdLoginResultProtector, BankIdLoginResultProtector>();

            services.AddHttpClient<IBankIdApiClient, BankIdApiClient>(ConfigureHttpClient)
                .ConfigurePrimaryHttpMessageHandler(() => GetHttpClientHandler(configureHttpClientHandler));

            services.TryAddTransient<IBankIdApiClient, BankIdApiClient>();
        }

        private static void ConfigureHttpClient(HttpClient httpClient)
        {
            httpClient.BaseAddress = BankIdUrls.ProdApiBaseUrl;
        }

        private static HttpClientHandler GetHttpClientHandler(Action<HttpClientHandler> configureHttpClientHandler)
        {
            var clientHandler = new HttpClientHandler
            {
                SslProtocols = SslProtocols.Tls12
            };
            configureHttpClientHandler?.Invoke(clientHandler);
            return clientHandler;
        }
    }
}