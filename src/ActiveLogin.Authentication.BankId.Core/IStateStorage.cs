using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.BankId.Core;

public readonly record struct StateKey(string Key)
{
    public static StateKey New() => new($"{Guid.NewGuid():N}");

    public static implicit operator string(StateKey key) => key.Key;
    public override string ToString() => Key;
};

public interface IStateStorage
{
    Task<T?> GetAsync<T>(StateKey key);
    Task<StateKey> SetAsync<T>(T value);
    Task<T> RemoveAsync<T>(StateKey key);
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
