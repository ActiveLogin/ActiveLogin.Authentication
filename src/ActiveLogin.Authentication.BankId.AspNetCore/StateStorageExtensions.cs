using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.Core;

namespace ActiveLogin.Authentication.BankId.AspNetCore;

internal static class StateStorageExtensions
{
    public static Task<T?> ReadAsync<T>(this IStateStorage stateStorage, StateKey key)
        where T : BankIdUiState
    {
        return stateStorage.ReadAsync(key).ContinueWith(t => t.Result as T);
    }

    public static Task<T?> RemoveAsync<T>(this IStateStorage stateStorage, StateKey key)
        where T : BankIdUiState
    {
        return stateStorage.RemoveAsync(key).ContinueWith(t => t.Result as T);
    }
}
