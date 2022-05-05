
using System.Net.Http.Headers;
using ActiveLogin.Authentication.GrandId.AspNetCore;

namespace Microsoft.Extensions.DependencyInjection;

internal static class GrandIdBuilderExtensions
{
    internal static IGrandIdBuilder UseUserAgent(this IGrandIdBuilder builder, ProductInfoHeaderValue productInfoHeaderValue)
    {
        builder.ConfigureHttpClient(httpClient =>
        {
            httpClient.DefaultRequestHeaders.UserAgent.Clear();
            httpClient.DefaultRequestHeaders.UserAgent.Add(productInfoHeaderValue);
        });

        return builder;
    }
}