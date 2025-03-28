using ActiveLogin.Authentication.BankId.Core;

using Microsoft.AspNetCore.Http;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.AspNetCore;

public class CookieStateStorage(
    IHttpContextAccessor httpContextAccessor
) : IStateStorage
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
    };

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
            // Deserialize the value from the cookie. The cookie value is a JSON string.
            var deserializedValue = JsonSerializer.Deserialize<T>(value, _jsonSerializerOptions);
            return Task.FromResult(deserializedValue);
        }

        return Task.FromResult<T?>(default);
    }

    public Task<bool> TryGetAsync<T>(StateKey key, [NotNullWhen(true)] out T? value)
    {
        var httpContext = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException(BankIdConstants.ErrorMessages.CouldNotAccessHttpContext);
        if (httpContext.Request.Cookies.TryGetValue(key, out var cookieValue))
        {
            value = JsonSerializer.Deserialize<T>(cookieValue, _jsonSerializerOptions);
            return Task.FromResult(true);
        }

        value = default;
        return Task.FromResult(false);
    }

    private Task<StateKey> Set<T>(StateKey key, T value)
    {
        var httpContext = _httpContextAccessor.HttpContext ?? throw new InvalidOperationException(BankIdConstants.ErrorMessages.CouldNotAccessHttpContext);
        var serializedValue = JsonSerializer.Serialize(value, _jsonSerializerOptions);
        httpContext.Response.Cookies.Append(key, serializedValue, _cookieOptions);

        return Task.FromResult(key);
    }

    public Task<StateKey> SetAsync<T>(T value)
    {
        var key = Guid.NewGuid().ToString();
        var stateKey = new StateKey(key);

        return Set(stateKey, value);
    }
}
