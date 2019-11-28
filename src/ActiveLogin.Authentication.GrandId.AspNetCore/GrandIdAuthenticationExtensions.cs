using System;
using System.Net.Http.Headers;
using System.Reflection;
using ActiveLogin.Authentication.GrandId.AspNetCore;
using Microsoft.AspNetCore.Authentication;

namespace Microsoft.Extensions.DependencyInjection
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
            var assemblyFileVersion = productAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
            var productVersion = assemblyFileVersion?.Version ?? "Unknown";

            return new ProductInfoHeaderValue(productName, productVersion);
        }
    }
}