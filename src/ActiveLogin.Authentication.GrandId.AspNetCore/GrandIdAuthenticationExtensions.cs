using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public static class GrandIdAuthenticationExtensions
    {
        public static AuthenticationBuilder AddGrandId(this AuthenticationBuilder builder)
        {
            return builder.AddGrandId(grandId =>
            {
                grandId
                    .UseDevelopmentEnvironment("GrandID", "Development")
                    .AddScheme("grandid-dev", "GrandID - Dev", new PathString("/signin-grandid-development"),  options => {});
            });
        }

        public static AuthenticationBuilder AddGrandId(this AuthenticationBuilder builder, Action<IGrandIdAuthenticationBuilder> grandId)
        {
            AddGrandIdServices(builder.Services);

            var grandIdAuthenticationBuilder = new GrandIdAuthenticationBuilder(builder);
            grandIdAuthenticationBuilder.AddDefaultServices();
            grandId(grandIdAuthenticationBuilder);

            return builder;
        }

        private static void AddGrandIdServices(IServiceCollection services)
        {
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<GrandIdAuthenticationOptions>, GrandIdAuthenticationPostConfigureOptions>());
        }
    }
}