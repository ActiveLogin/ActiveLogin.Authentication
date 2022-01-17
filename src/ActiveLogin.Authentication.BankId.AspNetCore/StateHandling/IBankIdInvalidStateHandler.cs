using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.BankId.AspNetCore.StateHandling
{
    /// <summary>
    /// Interface for handling invalid BankID state. This can happen when the user goes
    /// to the BankID login page without having a valid state cookie which usually
    /// is set as part of the auth challenge step.
    /// </summary>
    public interface IBankIdInvalidStateHandler
    {
        Task HandleAsync(HttpContext httpContext, BankIdInvalidStateContext invalidStateContext);
    }
}
