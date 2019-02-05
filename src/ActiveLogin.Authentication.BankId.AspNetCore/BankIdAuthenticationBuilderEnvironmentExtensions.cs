using System;
using ActiveLogin.Authentication.BankId.Api;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public static class BankIdAuthenticationBuilderEnvironmentExtensions
    {
        internal static IBankIdAuthenticationBuilder UseEnvironment(this IBankIdAuthenticationBuilder builder, Uri apiBaseUrl)
        {
            builder.EnableHttpClient();
            builder.ConfigureHttpClient(httpClient =>
            {
                httpClient.BaseAddress = apiBaseUrl;
            });

            builder.AuthenticationBuilder.Services.TryAddTransient<IBankIdApiClient, BankIdApiClient>();
            builder.AuthenticationBuilder.Services.TryAddTransient<IBankIdLauncher, BankIdLauncher>();

            return builder;
        }

        public static IBankIdAuthenticationBuilder UseTestEnvironment(this IBankIdAuthenticationBuilder builder)
        {
            return builder.UseEnvironment(BankIdUrls.TestApiBaseUrl);
        }

        public static IBankIdAuthenticationBuilder UseProductionEnvironment(this IBankIdAuthenticationBuilder builder)
        {
            return builder.UseEnvironment(BankIdUrls.ProductionApiBaseUrl);
        }

        public static IBankIdAuthenticationBuilder UseSimulatedEnvironment(this IBankIdAuthenticationBuilder builder)
            => UseSimulatedEnvironment(builder, x => new BankIdSimulatedApiClient());

        public static IBankIdAuthenticationBuilder UseSimulatedEnvironment(this IBankIdAuthenticationBuilder builder, string givenName, string surname)
            => UseSimulatedEnvironment(builder, x => new BankIdSimulatedApiClient(givenName, surname));

        public static IBankIdAuthenticationBuilder UseSimulatedEnvironment(this IBankIdAuthenticationBuilder builder, string givenName, string surname, string personalIdentityNumber)
            => UseSimulatedEnvironment(builder, x => new BankIdSimulatedApiClient(givenName, surname, personalIdentityNumber));

        private static IBankIdAuthenticationBuilder UseSimulatedEnvironment(this IBankIdAuthenticationBuilder builder, Func<IServiceProvider, IBankIdApiClient> bankIdDevelopmentApiClient)
        {
            builder.AuthenticationBuilder.Services.TryAddSingleton(bankIdDevelopmentApiClient);
            builder.AuthenticationBuilder.Services.TryAddTransient<IBankIdLauncher, BankIdDevelopmentLauncher>();

            return builder;
        }
    }
}
