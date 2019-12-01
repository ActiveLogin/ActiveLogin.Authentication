using System;
using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.AspNetCore;
using ActiveLogin.Authentication.BankId.AspNetCore.Launcher;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BankIdBuilderEnvironmentExtensions
    {
        internal static IBankIdBuilder UseEnvironment(this IBankIdBuilder builder, Uri apiBaseUrl)
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

        public static IBankIdBuilder UseTestEnvironment(this IBankIdBuilder builder)
        {
            return builder.UseEnvironment(BankIdUrls.TestApiBaseUrl);
        }

        public static IBankIdBuilder UseProductionEnvironment(this IBankIdBuilder builder)
        {
            return builder.UseEnvironment(BankIdUrls.ProductionApiBaseUrl);
        }

        public static IBankIdBuilder UseSimulatedEnvironment(this IBankIdBuilder builder)
            => UseSimulatedEnvironment(builder, x => new BankIdSimulatedApiClient());

        public static IBankIdBuilder UseSimulatedEnvironment(this IBankIdBuilder builder, string givenName, string surname)
            => UseSimulatedEnvironment(builder, x => new BankIdSimulatedApiClient(givenName, surname));

        public static IBankIdBuilder UseSimulatedEnvironment(this IBankIdBuilder builder, string givenName, string surname, string personalIdentityNumber)
            => UseSimulatedEnvironment(builder, x => new BankIdSimulatedApiClient(givenName, surname, personalIdentityNumber));

        private static IBankIdBuilder UseSimulatedEnvironment(this IBankIdBuilder builder, Func<IServiceProvider, IBankIdApiClient> bankIdDevelopmentApiClient)
        {
            builder.AuthenticationBuilder.Services.TryAddSingleton(bankIdDevelopmentApiClient);
            builder.AuthenticationBuilder.Services.TryAddTransient<IBankIdLauncher, BankIdDevelopmentLauncher>();

            return builder;
        }
    }
}
