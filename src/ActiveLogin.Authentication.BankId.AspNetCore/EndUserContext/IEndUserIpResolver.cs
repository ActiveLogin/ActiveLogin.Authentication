using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.BankId.AspNetCore.EndUserContext
{
    /// <summary>
    /// Resolves end user ip for the user.
    /// </summary>
    public interface IEndUserIpResolver
    {
        /// <summary>
        /// Return the end user IP for the user.
        /// </summary>
        /// <param name="httpContext">The context to extract end user IP from.</param>
        /// <returns></returns>
        string GetEndUserIp(HttpContext httpContext);
    }
}
