using System;
using System.Net.Http.Headers;
using System.Reflection;

using Microsoft.AspNetCore.Authentication;

namespace ActiveLogin.Authentication.GrandId.AspNetCore;

public static class GrandIdExtensions
{
    public static AuthenticationBuilder AddGrandId(this AuthenticationBuilder builder, Action<IGrandIdBuilder> grandId)
    {
        var grandIdBuilder = new GrandIdBuilder(builder);

        grandIdBuilder.UseUserAgent(GetActiveLoginUserAgent());

        grandId(grandIdBuilder);

        return builder;
    }

    private static ProductInfoHeaderValue GetActiveLoginUserAgent()
    {
        var productName = GrandIdConstants.ProductName;
        var productAssembly = typeof(GrandIdExtensions).Assembly;
        var assemblyFileVersion = productAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
        var productVersion = assemblyFileVersion?.Version ?? "Unknown";

        return new ProductInfoHeaderValue(productName, productVersion);
    }
}
