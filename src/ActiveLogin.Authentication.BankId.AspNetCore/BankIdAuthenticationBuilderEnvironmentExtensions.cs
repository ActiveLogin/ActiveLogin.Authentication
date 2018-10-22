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
            => UseDevelopmentEnvironment(builder, x => new BankIdDevelopmentApiClient());

        public static IBankIdAuthenticationBuilder UseDevelopmentEnvironment(this IBankIdAuthenticationBuilder builder, string givenName, string surname)
            => UseDevelopmentEnvironment(builder, x => new BankIdDevelopmentApiClient(givenName, surname));

        public static IBankIdAuthenticationBuilder UseDevelopmentEnvironment(this IBankIdAuthenticationBuilder builder, string givenName, string surname, string personalIdentityNumber)
            => UseDevelopmentEnvironment(builder, x => new BankIdDevelopmentApiClient(givenName, surname, personalIdentityNumber));

        private static IBankIdAuthenticationBuilder UseDevelopmentEnvironment(this IBankIdAuthenticationBuilder builder, Func<IServiceProvider, IBankIdApiClient> bankIdDevelopmentApiClient)
        {
            builder.AuthenticationBuilder.Services.TryAddSingleton(bankIdDevelopmentApiClient);
            builder.AuthenticationBuilder.Services.TryAddTransient<IBankIdLauncher, BankIdDevelopmentLauncher>();

            return builder;
        }
    }
}
