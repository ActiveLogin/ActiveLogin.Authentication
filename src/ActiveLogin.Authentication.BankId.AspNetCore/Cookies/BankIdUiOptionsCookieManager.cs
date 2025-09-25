using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Cookies;

internal class BankIdUiOptionsCookieManager(
    IHttpContextAccessor httpContextAccessor,
    IBankIdDataStateProtector<BankIdUiOptions> uiOptionsProtector
) : IBankIdUiOptionsCookieManager
{

    private static CookieOptions CookieOptions(TimeSpan duration) => new()
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Lax,
        IsEssential = true,
        MaxAge = duration,
    };

    public string Store(BankIdUiOptions uiOptions)
    {
        ArgumentNullException.ThrowIfNull(uiOptions);

        var httpContext = GetHttpContext();
        var guid = Guid.NewGuid().ToString("N"); // Use "N" format for shorter GUID without hyphens
        var cookieName = GetCookieName(guid);

        var protectedUiOptions = uiOptionsProtector.Protect(uiOptions);

        var cookieOptions = CookieOptions(BankIdConstants.UiOptionsCookieLifeTime);
        httpContext.Response.Cookies.Append(cookieName, protectedUiOptions, cookieOptions);

        return guid;
    }

    public BankIdUiOptions? Retrieve(string guid)
    {
        if (string.IsNullOrWhiteSpace(guid))
        {
            return null;
        }
        // If the GUID is not valid, attempt to unprotect it directly
        if (IsGuid(guid) == false)
        {
            return TryUnprotect(guid);
        }

        var httpContext = GetHttpContext();
        var cookieName = GetCookieName(guid);

        return httpContext.Request.Cookies.TryGetValue(cookieName, out var protectedValue)
            ? !string.IsNullOrWhiteSpace(protectedValue)
                ? uiOptionsProtector.Unprotect(protectedValue)
                : null
            : null;
    }

    public void Delete(string guid)
    {
        if (string.IsNullOrWhiteSpace(guid))
        {
            return;
        }

        var httpContext = GetHttpContext();
        var cookieName = GetCookieName(guid);

        httpContext.Response.Cookies.Delete(cookieName);
    }

    public bool IsGuid(string value)
    {
        return !string.IsNullOrWhiteSpace(value) && Guid.TryParse(value, out _);
    }

    private HttpContext GetHttpContext()
    {
        var httpContext = httpContextAccessor.HttpContext;
        return httpContext ?? throw new InvalidOperationException("HttpContext is not available.");
    }

    private static string GetCookieName(string guid)
    {
        return $"{BankIdConstants.DefaultUiOptionsCookieNamePrefix}{guid}";
    }

    private BankIdUiOptions? TryUnprotect(string protectedValue)
    {
        try
        {
            return uiOptionsProtector.Unprotect(protectedValue);
        }
        catch (Exception)
        {
            return null;
        }
    }
}
