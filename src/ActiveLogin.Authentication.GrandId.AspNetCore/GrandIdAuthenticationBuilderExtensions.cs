
using System.Net.Http.Headers;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    internal static class GrandIdAuthenticationBuilderExtensions
    {
        internal static IGrandIdAuthenticationBuilder UseUserAgent(this IGrandIdAuthenticationBuilder builder, ProductInfoHeaderValue productInfoHeaderValue)
        {
            builder.ConfigureHttpClient(httpClient =>
            {
                httpClient.DefaultRequestHeaders.UserAgent.Clear();
                httpClient.DefaultRequestHeaders.UserAgent.Add(productInfoHeaderValue);
            });

            return builder;
        }
    }
}
