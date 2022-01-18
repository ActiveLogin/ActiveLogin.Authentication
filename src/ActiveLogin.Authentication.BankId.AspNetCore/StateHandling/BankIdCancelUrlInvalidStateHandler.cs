using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.BankId.AspNetCore.StateHandling
{
    /// <summary>
    /// Redirect to the cancel return url if the state is invalid.
    /// </summary>
    public class BankIdCancelUrlInvalidStateHandler : IBankIdInvalidStateHandler
    {
        public Task HandleAsync(HttpContext httpContext, BankIdInvalidStateContext invalidStateContext)
        {
            httpContext.Response.Redirect(invalidStateContext.CancelReturnUrl);

            return Task.CompletedTask;
        }
    }
}
