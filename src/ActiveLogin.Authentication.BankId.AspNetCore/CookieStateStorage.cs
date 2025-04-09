using System.Diagnostics.CodeAnalysis;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using ActiveLogin.Authentication.BankId.Core;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;

namespace ActiveLogin.Authentication.BankId.AspNetCore;

public class CookieStateStorage(
    IHttpContextAccessor httpContextAccessor,
    ILogger<CookieStateStorage> logger,
    IServiceProvider serviceProvider
) : IStateStorage
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    private const int MaxCookieSize = 4096; // 4KB
    private readonly CookieOptions _cookieOptions = new()
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict,
        Path = "/",
        Expires = DateTimeOffset.UtcNow.AddMinutes(5) // Set expiration as needed
    };

    public Task<T?> GetAsync<T>(StateKey key)
    {
        var httpContext = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException(BankIdConstants.ErrorMessages.CouldNotAccessHttpContext);
        if (httpContext.Request.Cookies.TryGetValue(key, out var value))
        {
            var deserializedValue = Deserialize<T>(value);
            return Task.FromResult(deserializedValue);
        }

        return Task.FromResult<T?>(default);
    }

    public Task<bool> TryGetAsync<T>(StateKey key, [NotNullWhen(true)] out T? value)
    {
        value = default;
        var httpContext = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException(BankIdConstants.ErrorMessages.CouldNotAccessHttpContext);
        if (httpContext.Request.Cookies.TryGetValue(key, out var cookie))
        {
            value = Deserialize<T>(cookie);
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    private Task<StateKey> Set<T>(StateKey key, T value)
    {
        var httpContext = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException(BankIdConstants.ErrorMessages.CouldNotAccessHttpContext);
        var cookie = Serialize(value);

        if (cookie.Length > MaxCookieSize)
        {
            logger.LogWarning("Cookie size exceeds 4096 bytes. Consider using a different storage method.");
        }
        httpContext.Response.Cookies.Append(key, cookie, _cookieOptions);

        return Task.FromResult(key);
    }

    public Task<StateKey> SetAsync<T>(T value)
    {
        var stateKey = StateKey.New();

        return Set(stateKey, value);
    }

    private string Serialize<T>(T value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return serviceProvider
            .GetRequiredService<BankIdDataStateProtector<T>>()
            .Protect(value);
    }

    private T? Deserialize<T>(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        return serviceProvider
            .GetRequiredService<BankIdDataStateProtector<T>>()
            .Unprotect(value);
    }
}
