using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.Caching.Memory;

namespace ActiveLogin.Authentication.BankId.Core;

public class InMemoryStateStorage(
    IMemoryCache cache,
    TimeSpan slidingExpiration
) : IStateStorage
{
    public InMemoryStateStorage(IMemoryCache cache) : this(cache, TimeSpan.FromMinutes(5)) { }

    private readonly MemoryCacheEntryOptions _memoryCacheEntryOptions = new()
    {
        SlidingExpiration = slidingExpiration
    };

    public Task<T?> GetAsync<T>(StateKey key)
    {
        return cache.TryGetValue(key, out T? value)
            ? Task.FromResult(value)
            : Task.FromResult<T?>(default);
    }

    public Task<bool> TryGetAsync<T>(StateKey key, [NotNullWhen(true)] out T? value)
    {
        return cache.TryGetValue(key, out value)
            ? Task.FromResult(true)
            : Task.FromResult(false);
    }

    public Task<StateKey> SetAsync<T>(T value)
    {
        var key = Guid.NewGuid().ToString();
        var stateKey = new StateKey(key);
        cache.Set(stateKey, value, _memoryCacheEntryOptions);
        return Task.FromResult(stateKey);
    }
}
