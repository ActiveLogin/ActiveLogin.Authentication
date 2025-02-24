using System.Collections.Concurrent;

namespace ActiveLogin.Authentication.BankId.Core;

public class InMemoryStateStorage : IStateStorage
{
    private static readonly ConcurrentDictionary<string, object> _storage = new();

    public Task<StateKey> WriteAsync(object value)
    {
        var key = Guid.NewGuid().ToString();
        _ = _storage.TryAdd(key, value);
        var stateKey = new StateKey(key);
        return Task.FromResult(stateKey);
    }

    public Task<object?> RemoveAsync(StateKey key)
    {
        _ = _storage.TryRemove(key, out var value);
        return Task.FromResult(value);
    }

    public Task<object?> ReadAsync(StateKey key)
    {
        _ = _storage.TryGetValue(key, out var value);
        return Task.FromResult(value);
    }
}
