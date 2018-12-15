using System;
using Microsoft.AspNetCore.Authentication;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public static class GrandIdAuthenticationExtensions
    {
        public static AuthenticationBuilder AddGrandId(this AuthenticationBuilder builder, Action<IGrandIdAuthenticationBuilder> grandId)
        {
            var grandIdAuthenticationBuilder = new GrandIdAuthenticationBuilder(builder);
            grandId(grandIdAuthenticationBuilder);

            return builder;
        }
    }
}