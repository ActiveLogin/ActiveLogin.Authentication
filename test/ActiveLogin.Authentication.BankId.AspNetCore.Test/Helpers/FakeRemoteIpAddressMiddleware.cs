using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Test.Helpers
{
    internal class FakeRemoteIpAddressMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IPAddress _fakeAddress;

        /// <summary>
        /// Sets up a middleware that will fake the remote ip
        /// address of the request.
        /// </summary>
        /// <remarks>
        /// 192.0.2.0/24 is the recommended range of addresses.
        /// <see href="https://tools.ietf.org/html/rfc5737#section-1">RFC 5737</see>
        /// </remarks>
        /// <param name="next"></param>
        /// <param name="fakeAddress"></param>
        public FakeRemoteIpAddressMiddleware(RequestDelegate next, IPAddress fakeAddress)
        {
            this._next = next;
            this._fakeAddress = fakeAddress;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            httpContext.Connection.RemoteIpAddress = _fakeAddress;
            await this._next(httpContext);
        }
    }
}
