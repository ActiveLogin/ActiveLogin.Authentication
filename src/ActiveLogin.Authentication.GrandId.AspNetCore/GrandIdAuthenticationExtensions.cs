using System;
using Microsoft.AspNetCore.Authentication;
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
                    .AddSameDevice(options => { })
                    .AddOtherDevice(options => { });
            });
        }

        public static AuthenticationBuilder AddGrandId(this AuthenticationBuilder builder, Action<IGrandIdAuthenticationBuilder> grandId)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<GrandIdAuthenticationOptions>, GrandIdAuthenticationPostConfigureOptions>());

            var grandIdAuthenticationBuilder = new GrandIdAuthenticationBuilder(builder);
            grandIdAuthenticationBuilder.AddDefaultServices();
            grandId(grandIdAuthenticationBuilder);

            return builder;
        }
    }
}