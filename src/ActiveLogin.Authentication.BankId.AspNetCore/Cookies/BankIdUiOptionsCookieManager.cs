using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Cookies;

internal class BankIdUiOptionsCookieManager(
    IHttpContextAccessor httpContextAccessor,
    IBankIdUiOptionsProtector uiOptionsProtector
) : IBankIdUiOptionsCookieManager
{

    private CookieBuilder _uiOptionsCookieBuilder = new()
    {
        Name = BankIdConstants.DefaultUiOptionsCookieName,
        SecurePolicy = CookieSecurePolicy.SameAsRequest,
        HttpOnly = true,
        SameSite = SameSiteMode.Lax,
        IsEssential = true
    };

    public void Store(BankIdUiOptions uiOptions, DateTimeOffset expiresFrom)
    {
        ArgumentNullException.ThrowIfNull(uiOptions);

        var httpContext = GetHttpContext();
        var cookieOptions = _uiOptionsCookieBuilder.Build(httpContext, expiresFrom);
        var protectedUiOptions = uiOptionsProtector.Protect(uiOptions);

        httpContext.Response.Cookies.Append(BankIdConstants.DefaultUiOptionsCookieName, protectedUiOptions, cookieOptions);
    }

    public BankIdUiOptions? Retrieve()
    {
        var httpContext = GetHttpContext();

        return httpContext.Request.Cookies.TryGetValue(BankIdConstants.DefaultUiOptionsCookieName, out var protectedValue)
            ? !string.IsNullOrWhiteSpace(protectedValue)
                ? uiOptionsProtector.Unprotect(protectedValue)
                : null
            : null;
    }

    public void Delete()
    {
        var httpContext = GetHttpContext();
        httpContext.Response.Cookies.Delete(BankIdConstants.DefaultUiOptionsCookieName);
    }

    private HttpContext GetHttpContext()
    {
        var httpContext = httpContextAccessor.HttpContext;
        return httpContext ?? throw new InvalidOperationException("HttpContext is not available.");
    }
}
