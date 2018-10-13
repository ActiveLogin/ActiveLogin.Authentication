using System;
using ActiveLogin.Authentication.GrandId.Api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public static class GrandIdAuthenticationBuilderEnvironmentExtensions
    {
        public static IGrandIdAuthenticationBuilder UseEnvironment(this IGrandIdAuthenticationBuilder builder, Action<IGrandIdEnvironmentConfiguration> configureGrandIdEnvironment)
        {
            var configuration = new GrandIdEnvironmentConfiguration();
            configureGrandIdEnvironment(configuration);
            builder.ConfigureHttpClient(httpClient =>
            {
                httpClient.BaseAddress = configuration.ApiBaseUrl;
            });

            builder.AddGrandIdApiClient(configuration.ApiKey);

            return builder;
        }

        /// <summary>
        /// Configures the GrandID client to use the test endpoint of GrandID REST API.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="apiKey">The apiKey obtained from GrandID (Svensk E-identitet).</param>
        /// <returns></returns>
        public static IGrandIdAuthenticationBuilder UseTestEnvironment(this IGrandIdAuthenticationBuilder builder, string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentException($"The '{nameof(apiKey)}' must be provided.'", nameof(apiKey));
            }

            return builder.UseEnvironment(configuration =>
            {
                configuration.ApiBaseUrl = GrandIdUrls.TestApiBaseUrl;
                configuration.ApiKey = apiKey;
            });
        }

        /// <summary>
        /// Configures the GrandID client to use the production endpoint of GrandID REST API.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="apiKey">The apiKey obtained from GrandID (Svensk E-identitet).</param>
        /// <returns></returns>
        public static IGrandIdAuthenticationBuilder UseProductionEnvironment(this IGrandIdAuthenticationBuilder builder, string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentException($"The '{nameof(apiKey)}' must be provided.'", nameof(apiKey));
            }

            return builder.UseEnvironment(configuration =>
            {
                configuration.ApiBaseUrl = GrandIdUrls.ProductionApiBaseUrl;
                configuration.ApiKey = apiKey;
            });
        }

        /// <summary>
        /// Configures the GrandID client to an in memory implementation for development and/or test purposes.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IGrandIdAuthenticationBuilder UseDevelopmentEnvironment(this IGrandIdAuthenticationBuilder builder)
            => UseDevelopmentEnvironment(builder, x => new GrandIdDevelopmentApiClient());

        public static IGrandIdAuthenticationBuilder UseDevelopmentEnvironment(this IGrandIdAuthenticationBuilder builder, string givenName, string surname)
            => UseDevelopmentEnvironment(builder, x => new GrandIdDevelopmentApiClient(givenName, surname));

        public static IGrandIdAuthenticationBuilder UseDevelopmentEnvironment(this IGrandIdAuthenticationBuilder builder, string givenName, string surname, string personalIdentityNumber)
            => UseDevelopmentEnvironment(builder, x => new GrandIdDevelopmentApiClient(givenName, surname, personalIdentityNumber));
            

        private static IGrandIdAuthenticationBuilder UseDevelopmentEnvironment(this IGrandIdAuthenticationBuilder builder, Func<IServiceProvider, IGrandIdApiClient> grandIdDevelopmentApiClient)
        {
            builder.AuthenticationBuilder.Services.AddSingleton(grandIdDevelopmentApiClient);
            builder.AuthenticationBuilder.Services.PostConfigureAll<GrandIdAuthenticationOptions>(options =>
            {
                if (string.IsNullOrEmpty(options.AuthenticateServiceKey))
                {
                    options.AuthenticateServiceKey = "DEVELOPMENT";
                }
            });

            return builder;
        }

        private static IGrandIdAuthenticationBuilder AddGrandIdApiClient(this IGrandIdAuthenticationBuilder builder, string apiKey)
        {
            builder.AuthenticationBuilder.Services.TryAddTransient(x => new GrandIdApiClientConfiguration(apiKey));
            builder.AuthenticationBuilder.Services.TryAddTransient<IGrandIdApiClient, GrandIdApiClient>();

            return builder;
        }
    }
}