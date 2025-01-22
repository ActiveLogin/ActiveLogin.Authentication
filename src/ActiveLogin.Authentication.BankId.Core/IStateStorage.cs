namespace ActiveLogin.Authentication.BankId.Core;

public interface IStateStorage
{
    ValueTask<Guid> WriteAsync(object value);
    ValueTask<bool> TryReadAsync<T>(Guid key, out T? value);
    ValueTask<T?> RemoveAsync<T>(Guid key);
}
