using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.BankId.AspNetCore.EndUserContext
{
    /// <summary>
    /// Resolves the end user ip of the user from RemoteIpAddress of the connection.
    /// </summary>
    public class BankIdRemoteIpAddressEndUserIpResolver : IBankIdEndUserIpResolver
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
