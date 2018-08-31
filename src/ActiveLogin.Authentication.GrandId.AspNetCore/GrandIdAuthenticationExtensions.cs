using System;
using ActiveLogin.Authentication.Common.Serialization;
using ActiveLogin.Authentication.GrandId.Api;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public static class GrandIdAuthenticationExtensions
    {
        public static GrandIdAuthenticationBuilder AddGrandId(this AuthenticationBuilder builder)
        {
            return AddGrandId(
                builder,
                GrandIdAuthenticationDefaults.AuthenticationScheme,
                GrandIdAuthenticationDefaults.DisplayName,
                options => { }
            );
        }

        public static GrandIdAuthenticationBuilder AddGrandId(this AuthenticationBuilder builder, string authenticationScheme)
        {
            return AddGrandId(
                builder,
                authenticationScheme,
                GrandIdAuthenticationDefaults.DisplayName,
                options => { }
            );
        }

        public static GrandIdAuthenticationBuilder AddGrandId(this AuthenticationBuilder builder, string authenticationScheme, string displayName)
        {
            return AddGrandId(
                builder,
                authenticationScheme,
                displayName,
                options => { }
            );
        }

        public static GrandIdAuthenticationBuilder AddGrandId(this AuthenticationBuilder builder, Action<GrandIdAuthenticationOptions> configureOptions)
        {
            return AddGrandId(
                builder,
                GrandIdAuthenticationDefaults.AuthenticationScheme,
                GrandIdAuthenticationDefaults.DisplayName,
                configureOptions
            );
        }

        public static GrandIdAuthenticationBuilder AddGrandId(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<GrandIdAuthenticationOptions> configureOptions)
        {
            AddGrandIdServices(builder.Services);

            builder.AddScheme<GrandIdAuthenticationOptions, GrandIdAuthenticationHandler>(
                authenticationScheme,
                displayName,
                configureOptions
            );

            return new GrandIdAuthenticationBuilder(builder, authenticationScheme);
        }

        private static void AddGrandIdServices(IServiceCollection services)
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<GrandIdAuthenticationOptions>, GrandIdAuthenticationPostConfigureOptions>());

            services.TryAddSingleton<IJsonSerializer, SystemRuntimeJsonSerializer>();
        }

        public static IServiceCollection AddGrandIdDevelopmentEnvironment(this IServiceCollection services)
        {
            return AddGrandIdDevelopmentEnvironment(services, "GivenName", "Surname");
        }

        public static IServiceCollection AddGrandIdDevelopmentEnvironment(this IServiceCollection services, string givenName, string surname)
        {
            services.AddSingleton<IGrandIdApiClient>(x => new GrandIdDevelopmentApiClient(givenName, surname));
            return services;
        }
    }
}