using System;
using ActiveLogin.Authentication.GrandId.Api;
using Microsoft.Extensions.DependencyInjection;

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

        public static IGrandIdAuthenticationBuilder UseProdEnvironment(this IGrandIdAuthenticationBuilder builder, string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentException($"The '{nameof(apiKey)}' must be provided.'", nameof(apiKey));
            }

            return builder.UseEnvironment(configuration =>
            {
                configuration.ApiBaseUrl = GrandIdUrls.ProdApiBaseUrl;
                configuration.ApiKey = apiKey;
            });
        }

        public static IGrandIdAuthenticationBuilder UseDevelopmentEnvironment(this IGrandIdAuthenticationBuilder builder)
        {
            builder.AuthenticationBuilder.Services.AddSingleton<IGrandIdApiClient>(x => new GrandIdDevelopmentApiClient());

            return builder;
        }

        public static IGrandIdAuthenticationBuilder UseDevelopmentEnvironment(this IGrandIdAuthenticationBuilder builder, string givenName, string surname)
        {
            builder.AuthenticationBuilder.Services.AddSingleton<IGrandIdApiClient>(x => new GrandIdDevelopmentApiClient(givenName, surname));
            builder.AuthenticationBuilder.Services.PostConfigureAll<GrandIdAuthenticationOptions>(options =>
            {
                if (string.IsNullOrEmpty(options.AuthenticateServiceKey))
                {
                    options.AuthenticateServiceKey = "DEVELOPMENT";
                }
            });

            return builder;
        }

        public static IGrandIdAuthenticationBuilder UseDevelopmentEnvironment(this IGrandIdAuthenticationBuilder builder, string givenName, string surname, string personalIdentityNumber)
        {
            builder.AuthenticationBuilder.Services.AddSingleton<IGrandIdApiClient>(x => new GrandIdDevelopmentApiClient(givenName, surname, personalIdentityNumber));

            return builder;
        }
    }
}