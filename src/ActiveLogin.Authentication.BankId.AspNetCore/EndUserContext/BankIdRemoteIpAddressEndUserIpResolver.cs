using ActiveLogin.Authentication.BankId.Core.EndUserContext;

using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.BankId.AspNetCore.EndUserContext;

/// <summary>
/// Resolves the end user ip of the user from RemoteIpAddress of the connection.
/// </summary>
public class BankIdRemoteIpAddressEndUserIpResolver : IBankIdEndUserIpResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BankIdRemoteIpAddressEndUserIpResolver(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetEndUserIp()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var remoteIp = httpContext?.Connection.RemoteIpAddress;

        if(remoteIp == null)
        {
            return string.Empty;
        }

        if (remoteIp.IsIPv4MappedToIPv6)
        {
            return remoteIp.MapToIPv4().ToString();
        }

        return remoteIp.ToString();
    }
}
