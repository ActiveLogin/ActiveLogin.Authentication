using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    /// <summary>
    /// Resolve auth request user data.
    /// </summary>
    public interface IBankIdAuthRequestUserDataResolver
    {
        /// <summary>
        /// Returns the user data for the current context/request.
        /// </summary>
        /// <param name="authRequestContext">BankID auth request context.</param>
        /// <param name="httpContext">HttpContext for the current http request.</param>
        /// <returns></returns>
        Task<BankIdAuthUserData> GetUserDataAsync(BankIdAuthRequestContext authRequestContext, HttpContext httpContext);
    }
}
