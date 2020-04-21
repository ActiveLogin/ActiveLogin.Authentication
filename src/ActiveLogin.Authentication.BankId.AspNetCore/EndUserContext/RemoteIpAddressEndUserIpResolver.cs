using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.BankId.AspNetCore.EndUserContext
{
    public class RemoteIpAddressEndUserIpResolver : IEndUserIpResolver
    {
        public string GetEndUserIp(HttpContext httpContext)
        {
            var remoteIp = httpContext.Connection.RemoteIpAddress;

            if (remoteIp.IsIPv4MappedToIPv6)
            {
                return remoteIp.MapToIPv4().ToString();
            }

            return remoteIp.ToString();
        }
    }
}
