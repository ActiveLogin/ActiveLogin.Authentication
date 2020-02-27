using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.BankId.AspNetCore.EndUserContext
{
    public interface IEndUserIpResolver
    {
        string GetEndUserIp(HttpContext httpContext);
    }
}
