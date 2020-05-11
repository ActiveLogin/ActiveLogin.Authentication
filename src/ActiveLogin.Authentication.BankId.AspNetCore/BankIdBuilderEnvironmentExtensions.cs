using System;
using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.AspNetCore;
using ActiveLogin.Authentication.BankId.AspNetCore.Launcher;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BankIdBuilderEnvironmentExtensions
    {
        internal static IBankIdBuilder UseEnvironment(this IBankIdBuilder builder, Uri apiBaseUrl, string environment)
        {
            SetActiveLoginContext(builder.AuthenticationBuilder.Services, environment, BankIdConstants.BankIdApiVersion);

            builder.ConfigureHttpClient(httpClient =>
            {
                httpClient.BaseAddress = apiBaseUrl;
            });

            builder.AuthenticationBuilder.Services.AddTransient<IBankIdLauncher, BankIdLauncher>();

            if (builder is BankIdBuilder bankIdBuilder)
            {
                bankIdBuilder.EnableHttpBankIdApiClient();
            }

            return builder;
        }

        public static IBankIdBuilder UseTestEnvironment(this IBankIdBuilder builder)
        {
            return builder.UseEnvironment(BankIdUrls.TestApiBaseUrl, BankIdEnvironments.Test);
        }

        public static IBankIdBuilder UseProductionEnvironment(this IBankIdBuilder builder)
        {
            return builder.UseEnvironment(BankIdUrls.ProductionApiBaseUrl, BankIdEnvironments.Production);
        }

        public static IBankIdBuilder UseSimulatedEnvironment(this IBankIdBuilder builder)
            => UseSimulatedEnvironment(builder, x => new BankIdSimulatedApiClient());

        public static IBankIdBuilder UseSimulatedEnvironment(this IBankIdBuilder builder, string givenName, string surname)
            => UseSimulatedEnvironment(builder, x => new BankIdSimulatedApiClient(givenName, surname));

        public static IBankIdBuilder UseSimulatedEnvironment(this IBankIdBuilder builder, string givenName, string surname, string personalIdentityNumber)
            => UseSimulatedEnvironment(builder, x => new BankIdSimulatedApiClient(givenName, surname, personalIdentityNumber));

        private static IBankIdBuilder UseSimulatedEnvironment(this IBankIdBuilder builder, Func<IServiceProvider, IBankIdApiClient> bankIdDevelopmentApiClient)
        {
            SetActiveLoginContext(builder.AuthenticationBuilder.Services, BankIdEnvironments.Simulated, BankIdConstants.BankIdApiVersion);

            builder.AuthenticationBuilder.Services.AddSingleton(bankIdDevelopmentApiClient);
            builder.AuthenticationBuilder.Services.AddSingleton<IBankIdLauncher, BankIdDevelopmentLauncher>();

            return builder;
        }

        private static void SetActiveLoginContext(IServiceCollection services, string environment, string apiVersion)
        {
            services.Configure<BankIdActiveLoginContext>(context =>
            {
                context.BankIdApiEnvironment = environment;
                context.BankIdApiVersion = apiVersion;
            });
        }
    }
}
