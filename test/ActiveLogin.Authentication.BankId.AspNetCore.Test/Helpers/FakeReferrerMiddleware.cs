#nullable enable
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Test.Helpers;

internal class FakeReferrerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _referrer;

    /// <summary>
    /// Sets up a middleware that will fake the referrer header of the request.
    /// </summary>
    /// <remarks>
    /// E.g. "https://localhost:3000", "http://localhost", "https://example.com"
    /// </remarks>
    /// <param name="next"></param>
    /// <param name="fakeReferrer"></param>
    public FakeReferrerMiddleware(RequestDelegate next, string? fakeReferrer)
    {
        _next = next;
        _referrer = fakeReferrer ?? "http://localhost";
    }

    public async Task Invoke(HttpContext httpContext)
    {
        httpContext.Request.Headers[HeaderNames.Referer] = _referrer;
        await _next(httpContext);
    }

    public static string DefaultUserAgent =>
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/132.0.0.0 Safari/537.36 Edg/132.0.0.0";
}
