using System.Collections.Concurrent;

namespace ActiveLogin.Authentication.BankId.Core;

public class InMemoryStateStorage : IStateStorage
{
    private static readonly ConcurrentDictionary<Guid, object> _storage = new();

    public ValueTask<Guid> WriteAsync(object value)
    {
        var key = Guid.NewGuid();
        _ = _storage.TryAdd(key, value);
        return new ValueTask<Guid>(key);
    }

    public ValueTask<T?> RemoveAsync<T>(Guid key)
    {
        return _storage.TryRemove(key, out var value)
            ? new ValueTask<T?>((T)value)
            : new ValueTask<T?>(default(T));
    }

    public ValueTask<bool> TryReadAsync<T>(Guid key, out T? value)
    {
        if (_storage.TryGetValue(key, out var obj))
        {
            value = (T)obj;
            return new ValueTask<bool>(true);
        }

        value = default;
        return new ValueTask<bool>(false);
    }
}
