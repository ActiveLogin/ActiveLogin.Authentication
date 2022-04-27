using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.BankId.AspNetCore.EndUserContext
{
    /// <summary>
    /// Resolves end user ip for the user.
    /// </summary>
    public interface IBankIdEndUserIpResolver
    {
        /// <summary>
        /// Return the end user IP for the user.
        /// </summary>
        /// <returns></returns>
        string GetEndUserIp();
    }
}
