using System;
using System.Net.Http.Headers;
using System.Reflection;
using Microsoft.AspNetCore.Authentication;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public static class GrandIdAuthenticationExtensions
    {
        public static AuthenticationBuilder AddGrandId(this AuthenticationBuilder builder, Action<IGrandIdAuthenticationBuilder> grandId)
        {
            var grandIdAuthenticationBuilder = new GrandIdAuthenticationBuilder(builder);

            grandIdAuthenticationBuilder.UseUserAgent(GetActiveLoginUserAgent());

            grandId(grandIdAuthenticationBuilder);

            return builder;
        }

        private static ProductInfoHeaderValue GetActiveLoginUserAgent()
        {
            var productName = GrandIdAuthenticationConstants.ProductName;
            var productAssembly = typeof(GrandIdAuthenticationExtensions).Assembly;
            var productVersion = productAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;

            return new ProductInfoHeaderValue(productName, productVersion);
        }
    }
}