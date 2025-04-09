using System.Diagnostics.CodeAnalysis;

using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.BankId.Core;

public readonly record struct StateKey(string Key)
{
    public static StateKey New() => new(Guid.NewGuid().ToString());
    public StateKey(Guid guid) : this(guid.ToString()) { }

    public static implicit operator string(StateKey key) => key.Key;
};

public interface IStateStorage
{
    Task<T?> GetAsync<T>(StateKey key);
    Task<bool> TryGetAsync<T>(StateKey key, [NotNullWhen(true)] out T? value);
    Task<StateKey> SetAsync<T>(T value);
    Task<StateKey> SetAsync<T>(StateKey key, T value);
}

public static class BankIdBuilderStateStorageExtensions
{
    public static IBankIdBuilder AddStateStorage<TStateStorage>(this IBankIdBuilder builder)
        where TStateStorage : class, IStateStorage
    {
        builder.Services.AddSingleton<IStateStorage, TStateStorage>();
        return builder;
    }

    public static IBankIdBuilder AddStateStorage(this IBankIdBuilder builder, IStateStorage stateStorage)
    {
        builder.Services.AddSingleton(stateStorage);
        return builder;
    }
}
