using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.Caching.Memory;

namespace ActiveLogin.Authentication.BankId.Core;

public class InMemoryStateStorage(IMemoryCache cache, TimeSpan slidingExpiration) : IStateStorage
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
    private Task<StateKey> Set<T>(T value, MemoryCacheEntryOptions options)
    {
        var key = Guid.NewGuid().ToString();
        var stateKey = new StateKey(key);
        cache.Set(stateKey, value, options);
        return Task.FromResult(stateKey);
    }
    public Task<StateKey> SetAsync<T>(T value) => Set(value, _memoryCacheEntryOptions);
    public Task<StateKey> SetAsync<T>(T value, Action<StateKey, T> evictionCallback)
    {
        var options = new MemoryCacheEntryOptions()
        {
            SlidingExpiration = _memoryCacheEntryOptions.SlidingExpiration
        };
        options.RegisterPostEvictionCallback((key, value, reason, state) =>
        {
            evictionCallback((StateKey)key, (T)value!);
        });

        return Set(value, options);
    }
}
