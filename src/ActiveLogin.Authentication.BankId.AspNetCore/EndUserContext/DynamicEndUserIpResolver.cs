using System;
using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.BankId.AspNetCore.EndUserContext
{
    internal class DynamicEndUserIpResolver : IEndUserIpResolver
    {
        private readonly Func<HttpContext, string> _resolver;

        public DynamicEndUserIpResolver(Func<HttpContext, string> resolver)
        {
            _resolver = resolver;
        }

        public string GetEndUserIp(HttpContext httpContext)
        {
            return _resolver(httpContext);
        }
    }
}
