using ActiveLogin.Authentication.Common.Serialization;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public static class GrandIdAuthenticationBuilderExtensions
    {
        internal static IGrandIdAuthenticationBuilder AddDefaultServices(this IGrandIdAuthenticationBuilder builder)
        {
            builder.AuthenticationBuilder.Services.TryAddSingleton<IJsonSerializer, SystemRuntimeJsonSerializer>();

            return builder;
        }
    }
}