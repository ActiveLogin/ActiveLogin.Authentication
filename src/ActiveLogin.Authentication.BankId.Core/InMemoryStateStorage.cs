using System.Collections.Concurrent;

namespace ActiveLogin.Authentication.BankId.Core;

public interface IStateStorage<T>
    where T : class
{
    Task<StateKey> WriteAsync(T value);
    Task<T?> ReadAsync(StateKey key);
    Task<T?> RemoveAsync(StateKey key);
}

public record struct StateKey(string Key)
{
    public static implicit operator string(StateKey key) => key.Key;
};


public class InMemoryStateStorage<T> : IStateStorage<T>
    where T : class
{
    private static readonly ConcurrentDictionary<string, T> _storage = new();

    public Task<StateKey> WriteAsync(T value)
    {
        var key = Guid.NewGuid().ToString();
        _ = _storage.TryAdd(key, value);
        var stateKey = new StateKey(key);
        return Task.FromResult(stateKey);
    }

    public Task<T?> RemoveAsync(StateKey key)
    {
        _ = _storage.TryRemove(key, out var value);
        return Task.FromResult(value);
    }

    public Task<T?> ReadAsync(StateKey key)
    {
        _ = _storage.TryGetValue(key, out var value);
        return Task.FromResult(value);
    }
}
