using System;
using ActiveLogin.Authentication.GrandId.Api;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public static class GrandIdAuthenticationBuilderEnvironmentExtensions
    {
        private static IGrandIdAuthenticationBuilder UseEnvironment(this IGrandIdAuthenticationBuilder builder, Uri apiBaseUrl, Action<IGrandIdEnvironmentConfiguration> configuration)
        {
            var environmentConfiguration = new GrandIdEnvironmentConfiguration();
            configuration(environmentConfiguration);

            if (string.IsNullOrEmpty(environmentConfiguration.ApiKey))
            {
                throw new InvalidOperationException($"A valid '{nameof(environmentConfiguration.ApiKey)}' must be provided.'");
            }

            builder.EnableHttpClient();
            builder.ConfigureHttpClient(httpClient =>
            {
                httpClient.BaseAddress = apiBaseUrl;
            });

            builder.AddGrandIdApiClient(environmentConfiguration.ApiKey, environmentConfiguration.BankIdServiceKey);

            return builder;
        }

        /// <summary>
        /// Configures the GrandID client to use the test endpoint of GrandID REST API.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configuration">Configure GrandID.</param>
        /// <returns></returns>
        public static IGrandIdAuthenticationBuilder UseTestEnvironment(this IGrandIdAuthenticationBuilder builder, Action<IGrandIdEnvironmentConfiguration> configuration)
        {
            return builder.UseEnvironment(GrandIdUrls.TestApiBaseUrl, configuration);
        }

        /// <summary>
        /// Configures the GrandID client to use the production endpoint of GrandID REST API.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configuration">Configure GrandID.</param>
        /// <returns></returns>
        public static IGrandIdAuthenticationBuilder UseProductionEnvironment(this IGrandIdAuthenticationBuilder builder, Action<IGrandIdEnvironmentConfiguration> configuration)
        {
            return builder.UseEnvironment(GrandIdUrls.ProductionApiBaseUrl, configuration);
        }

        /// <summary>
        /// Configures the GrandID client to a simulated, in memory implementation for development and/or test purposes.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IGrandIdAuthenticationBuilder UseSimulatedEnvironment(this IGrandIdAuthenticationBuilder builder)
            => UseSimulatedEnvironment(builder, x => new GrandIdSimulatedApiClient());

        public static IGrandIdAuthenticationBuilder UseSimulatedEnvironment(this IGrandIdAuthenticationBuilder builder, string givenName, string surname)
            => UseSimulatedEnvironment(builder, x => new GrandIdSimulatedApiClient(givenName, surname));

        public static IGrandIdAuthenticationBuilder UseSimulatedEnvironment(this IGrandIdAuthenticationBuilder builder, string givenName, string surname, string personalIdentityNumber)
            => UseSimulatedEnvironment(builder, x => new GrandIdSimulatedApiClient(givenName, surname, personalIdentityNumber));


        private static IGrandIdAuthenticationBuilder UseSimulatedEnvironment(this IGrandIdAuthenticationBuilder builder, Func<IServiceProvider, IGrandIdApiClient> grandIdDevelopmentApiClient)
        {
            builder.AuthenticationBuilder.Services.TryAddSingleton(grandIdDevelopmentApiClient);

            return builder;
        }

        private static IGrandIdAuthenticationBuilder AddGrandIdApiClient(this IGrandIdAuthenticationBuilder builder, string apiKey, string bankIdServiceKey)
        {
            builder.AuthenticationBuilder.Services.TryAddTransient(x => new GrandIdApiClientConfiguration(apiKey, bankIdServiceKey));
            builder.AuthenticationBuilder.Services.TryAddTransient<IGrandIdApiClient, GrandIdApiClient>();

            return builder;
        }
    }
}