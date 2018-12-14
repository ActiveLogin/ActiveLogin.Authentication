using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public static class GrandIdAuthenticationExtensions
    {
        public static AuthenticationBuilder AddGrandId(this AuthenticationBuilder builder, Action<IGrandIdAuthenticationBuilder> grandId)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<GrandIdBankIdAuthenticationOptions>, GrandIdBankIdAuthenticationPostConfigureOptions>());

            var grandIdAuthenticationBuilder = new GrandIdAuthenticationBuilder(builder);
            grandId(grandIdAuthenticationBuilder);

            return builder;
        }
    }
}