using System;
using ActiveLogin.Authentication.Common.Serialization;
using ActiveLogin.Authentication.GrandId.Api;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public static class GrandIdAuthenticationBuilderExtensions
    {
        public static IGrandIdAuthenticationBuilder AddDefaultServices(this IGrandIdAuthenticationBuilder builder)
        {
            builder.AuthenticationBuilder.Services.TryAddSingleton<IJsonSerializer, SystemRuntimeJsonSerializer>();

            return builder;
        }

        public static IGrandIdAuthenticationBuilder AddGrandIdApiClient(this IGrandIdAuthenticationBuilder builder, string apiKey)
        {
            builder.AuthenticationBuilder.Services.TryAddTransient(x => new GrandIdApiClientConfiguration(apiKey));
            builder.AuthenticationBuilder.Services.TryAddTransient<IGrandIdApiClient, GrandIdApiClient>();

            return builder;
        }
    }
}