using System;
using System.Net.Http;
using ActiveLogin.Authentication.GrandId.Api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public static class GrandIdAuthenticationBuilderExtensions
    {
        public static GrandIdAuthenticationBuilder AddGrandIdEnvironmentConfiguration(this GrandIdAuthenticationBuilder builder, Action<GrandIdEnvironmentConfiguration> configureGrandIdEnvironment)
        {
            var configuration = new GrandIdEnvironmentConfiguration();
            configureGrandIdEnvironment(configuration);
            builder.ConfigureGrandIdHttpClient(httpClient =>
            {
                httpClient.BaseAddress = configuration.ApiBaseUrl;
            });
            return builder;
        }

        public static GrandIdAuthenticationBuilder AddGrandIdTestEnvironment(this GrandIdAuthenticationBuilder builder)
        {
            builder.AddGrandIdEnvironmentConfiguration(configuration =>
            {
                configuration.ApiBaseUrl = GrandIdUrls.TestApiBaseUrl;
            });

            return builder;
        }

        public static GrandIdAuthenticationBuilder AddGrandIdProdEnvironment(this GrandIdAuthenticationBuilder builder)
        {
            builder.AddGrandIdEnvironmentConfiguration(configuration =>
            {
                configuration.ApiBaseUrl = GrandIdUrls.ProdApiBaseUrl;
            });

            return builder;
        }
    }
}