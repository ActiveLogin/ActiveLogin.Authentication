using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.BankId.AspNetCore.StateHandling;

/// <summary>
/// Redirect to the cancel return url if the state is invalid.
/// </summary>
public class BankIdCancelUrlInvalidStateHandler : IBankIdInvalidStateHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BankIdCancelUrlInvalidStateHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task HandleAsync(BankIdInvalidStateContext invalidStateContext)
    {
        _httpContextAccessor.HttpContext?.Response.Redirect(invalidStateContext.CancelReturnUrl);

        return Task.CompletedTask;
    }
}
