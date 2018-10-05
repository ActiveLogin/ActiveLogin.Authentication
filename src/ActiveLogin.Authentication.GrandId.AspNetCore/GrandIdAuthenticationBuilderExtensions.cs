using System;
using ActiveLogin.Authentication.Common.Serialization;
using ActiveLogin.Authentication.GrandId.Api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public static class GrandIdAuthenticationBuilderExtensions
    {
        public static GrandIdAuthenticationBuilder AddDefaultServices(this GrandIdAuthenticationBuilder builder)
        {
            builder.AuthenticationBuilder.Services.TryAddSingleton<IJsonSerializer, SystemRuntimeJsonSerializer>();

            return builder;
        }

        public static GrandIdAuthenticationBuilder AddGrandIdApiClient(this GrandIdAuthenticationBuilder builder, string apiKey)
        {
            builder.AuthenticationBuilder.Services.TryAddTransient(x => new GrandIdApiClientConfiguration(apiKey));
            builder.AuthenticationBuilder.Services.TryAddTransient<IGrandIdApiClient, GrandIdApiClient>();

            return builder;
        }



        public static GrandIdAuthenticationBuilder UseEnvironment(this GrandIdAuthenticationBuilder builder, Action<GrandIdEnvironmentConfiguration> configureGrandIdEnvironment)
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

        public static GrandIdAuthenticationBuilder UseTestEnvironment(this GrandIdAuthenticationBuilder builder, string apiKey)
        {
            builder.UseEnvironment(configuration =>
            {
                configuration.ApiBaseUrl = GrandIdUrls.TestApiBaseUrl;
                configuration.ApiKey = apiKey;
            });

            return builder;
        }

        public static GrandIdAuthenticationBuilder UseProdEnvironment(this GrandIdAuthenticationBuilder builder, string apiKey)
        {
            builder.UseEnvironment(configuration =>
            {
                configuration.ApiBaseUrl = GrandIdUrls.ProdApiBaseUrl;
                configuration.ApiKey = apiKey;
            });

            return builder;
        }

        public static GrandIdAuthenticationBuilder UseDevelopmentEnvironment(this GrandIdAuthenticationBuilder builder)
        {
            builder.AuthenticationBuilder.Services.AddSingleton<IGrandIdApiClient>(x => new GrandIdDevelopmentApiClient());

            return builder;
        }

        public static GrandIdAuthenticationBuilder UseDevelopmentEnvironment(this GrandIdAuthenticationBuilder builder, string givenName, string surname)
        {
            builder.AuthenticationBuilder.Services.AddSingleton<IGrandIdApiClient>(x => new GrandIdDevelopmentApiClient(givenName, surname));

            return builder;
        }

        public static GrandIdAuthenticationBuilder UseDevelopmentEnvironment(this GrandIdAuthenticationBuilder builder, string givenName, string surname, string personalIdentityNumber)
        {
            builder.AuthenticationBuilder.Services.AddSingleton<IGrandIdApiClient>(x => new GrandIdDevelopmentApiClient(givenName, surname, personalIdentityNumber));

            return builder;
        }



        public static GrandIdAuthenticationBuilder AddScheme(this GrandIdAuthenticationBuilder builder)
        {
            return AddScheme(
                builder,
                GrandIdAuthenticationDefaults.AuthenticationScheme,
                GrandIdAuthenticationDefaults.DisplayName,
                options => { }
            );
        }

        public static GrandIdAuthenticationBuilder AddScheme(this GrandIdAuthenticationBuilder builder, string authenticationScheme)
        {
            return AddScheme(
                builder,
                authenticationScheme,
                GrandIdAuthenticationDefaults.DisplayName,
                options => { }
            );
        }

        public static GrandIdAuthenticationBuilder AddScheme(this GrandIdAuthenticationBuilder builder, string authenticationScheme, string displayName)
        {
            return AddScheme(
                builder,
                authenticationScheme,
                displayName,
                options => { }
            );
        }

        public static GrandIdAuthenticationBuilder AddScheme(this GrandIdAuthenticationBuilder builder, Action<GrandIdAuthenticationOptions> configureOptions)
        {
            return AddScheme(
                builder,
                GrandIdAuthenticationDefaults.AuthenticationScheme,
                GrandIdAuthenticationDefaults.DisplayName,
                configureOptions
            );
        }

        public static GrandIdAuthenticationBuilder AddScheme(this GrandIdAuthenticationBuilder builder, string authenticationScheme, string displayName, Action<GrandIdAuthenticationOptions> configureOptions)
        {
            builder.AuthenticationBuilder.AddScheme<GrandIdAuthenticationOptions, GrandIdAuthenticationHandler>(
                authenticationScheme,
                displayName,
                configureOptions
            );

            return builder;
        }
    }
}