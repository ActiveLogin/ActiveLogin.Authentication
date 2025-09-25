using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using ActiveLogin.Authentication.BankId.Core;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;

namespace ActiveLogin.Authentication.BankId.AspNetCore;

/// <summary>
/// State storage using cookies.
/// Requires IServiceProvider to dynamically resolve BankIdDataStateProtector.
/// </summary>
/// <param name="serviceProvider"></param>
internal class CookieStateStorage(
    IServiceProvider serviceProvider
) : IStateStorage
{
    internal static string StateCookieName(StateKey stateKey) => $"{BankIdConstants.StateKeyPrefix}{stateKey}";

    private readonly IHttpContextAccessor httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
    private readonly ILogger<CookieStateStorage> logger = serviceProvider.GetRequiredService<ILogger<CookieStateStorage>>();

    private const int MaxCookieSize = 4096; // 4KB

    public Task<T?> GetAsync<T>(StateKey key)
    {
        var httpContext = httpContextAccessor.HttpContext ?? throw new InvalidOperationException(BankIdConstants.ErrorMessages.CouldNotAccessHttpContext);
        if (httpContext.Request.Cookies.TryGetValue(StateCookieName(key), out var value) && !string.IsNullOrEmpty(value))
        {
            var deserializedValue = Deserialize<T>(value);
            return Task.FromResult<T?>(deserializedValue);
        }

        return Task.FromResult<T?>(default);
    }

    public Task<StateKey> SetAsync<T>(T value)
    {
        var stateKey = StateKey.New();

        var key = Set(stateKey, value);
        return Task.FromResult(key);
    }

    public async Task<T> RemoveAsync<T>(StateKey key)
    {
        var httpContext = httpContextAccessor.HttpContext ?? throw new InvalidOperationException(BankIdConstants.ErrorMessages.CouldNotAccessHttpContext);
        var cookieName = StateCookieName(key);
        httpContext.Response.Cookies.Delete(BankIdConstants.StateKeyCookieName);
        if (httpContext.Request.Cookies.TryGetValue(cookieName, out var value) && !string.IsNullOrEmpty(value))
        {
            var deserializedValue = Deserialize<T>(value);
            httpContext.Response.Cookies.Delete(cookieName);
            return await Task.FromResult(deserializedValue);
        }

        throw new InvalidOperationException("Could not find state in cookies to remove.");
    }

    private StateKey Set<T>(StateKey key, T value)
    {
        var httpContext = httpContextAccessor.HttpContext ?? throw new InvalidOperationException(BankIdConstants.ErrorMessages.CouldNotAccessHttpContext);
        var cookie = Serialize(value);

        if (cookie.Length > MaxCookieSize)
        {
            logger.LogWarning("Cookie size exceeds 4096 bytes. Consider using a different storage method.");
        }

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = httpContext.Request.IsHttps,
            SameSite = SameSiteMode.Strict,
            Path = "/",
            IsEssential = true,
            MaxAge = TimeSpan.FromMinutes(5)
        };

        httpContext.Response.Cookies.Append(StateCookieName(key), cookie, cookieOptions);

        return key;
    }

    private string Serialize<T>(T value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return serviceProvider
            .GetRequiredService<IBankIdDataStateProtector<T>>()
            .Protect(value);
    }

    private T Deserialize<T>(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        return serviceProvider
            .GetRequiredService<IBankIdDataStateProtector<T>>()
            .Unprotect(value);
    }
}
