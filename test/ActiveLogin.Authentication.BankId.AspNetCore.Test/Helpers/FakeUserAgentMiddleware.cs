#nullable enable
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

using Microsoft.Net.Http.Headers;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Test.Helpers;
internal class FakeUserAgentMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _fakeUserAgent;

    /// <summary>
    /// Sets up a middleware that will fake the User-Agent
    /// header of the request.
    /// </summary>
    /// <remarks>
    /// E.g. "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/132.0.0.0 Safari/537.36 Edg/132.0.0.0"
    /// <see href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/User-Agent">User-Agent docs.</see>
    /// </remarks>
    /// <param name="next"></param>
    /// <param name="fakeUserAgent"></param>
    public FakeUserAgentMiddleware(RequestDelegate next, string fakeUserAgent)
    {
        _next = next;
        _fakeUserAgent = fakeUserAgent;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        httpContext.Request.Headers[HeaderNames.UserAgent] = _fakeUserAgent;
        await _next(httpContext);
    }

    public static string DefaultUserAgent =>
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/132.0.0.0 Safari/537.36 Edg/132.0.0.0";
}
