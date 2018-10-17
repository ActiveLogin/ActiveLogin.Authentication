using System;
using ActiveLogin.Authentication.BankId.Api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public static class BankIdAuthenticationBuilderEnvironmentExtensions
    {
        public static IBankIdAuthenticationBuilder UseEnvironment(this IBankIdAuthenticationBuilder builder, Action<BankIdEnvironmentConfiguration> configureBankIdEnvironment)
        {
            var configuration = new BankIdEnvironmentConfiguration();
            configureBankIdEnvironment(configuration);
            builder.ConfigureBankIdHttpClient(httpClient =>
            {
                httpClient.BaseAddress = configuration.ApiBaseUrl;
            });

            builder.AuthenticationBuilder.Services.TryAddTransient<IBankIdApiClient, BankIdApiClient>();
            builder.AuthenticationBuilder.Services.TryAddTransient<IBankIdLauncher, BankIdLauncher>();

            return builder;
        }

        public static IBankIdAuthenticationBuilder UseTestEnvironment(this IBankIdAuthenticationBuilder builder)
        {
            builder.UseEnvironment(configuration =>
            {
                configuration.ApiBaseUrl = BankIdUrls.TestApiBaseUrl;
            });

            return builder;
        }

        public static IBankIdAuthenticationBuilder UseProductionEnvironment(this IBankIdAuthenticationBuilder builder)
        {
            builder.UseEnvironment(configuration =>
            {
                configuration.ApiBaseUrl = BankIdUrls.ProdApiBaseUrl;
            });

            return builder;
        }

        public static IBankIdAuthenticationBuilder UseDevelopmentEnvironment(this IBankIdAuthenticationBuilder builder)
        {
            builder.AuthenticationBuilder.Services.AddSingleton<IBankIdApiClient>(x => new BankIdDevelopmentApiClient());
            AddDevelopmentServices(builder.AuthenticationBuilder.Services);

            return builder;
        }

        public static IBankIdAuthenticationBuilder UseDevelopmentEnvironment(this IBankIdAuthenticationBuilder builder, string givenName, string surname)
        {
            builder.AuthenticationBuilder.Services.AddSingleton<IBankIdApiClient>(x => new BankIdDevelopmentApiClient(givenName, surname));
            AddDevelopmentServices(builder.AuthenticationBuilder.Services);

            return builder;
        }

        private static void AddDevelopmentServices(IServiceCollection services)
        {
            services.AddSingleton<IBankIdLauncher, BankIdDevelopmentLauncher>();
        }
    }
}