using ActiveLogin.Authentication.BankId.Core.UserContext.Device;

using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.BankId.Core.UserContext.Device.Configuration;

/// <inheritdoc cref="IBankIdEndUserDeviceConfigurationBuilder"/>
public class BankIdEndUserDeviceConfigurationBuilder(IServiceCollection services) : IBankIdEndUserDeviceConfigurationBuilder
{
    public BankIdEndUserDeviceType DeviceType { get; set; } = BankIdEndUserDeviceType.Web;

    public void UseResolverFactory<T>() where T : IBankIdEndUserDeviceDataResolverFactory
    {
        if (typeof(T).IsInterface)
        {
            throw new ArgumentException("T must be a class implementing IBankIdEndUserDeviceDataResolverFactory");
        }

        ResolverFactory = typeof(T);
    }

    public void AddDeviceResolver<T>() where T : IBankIdEndUserDeviceDataResolver
    {
        if (typeof(T).IsInterface)
        {
            throw new ArgumentException("T must be a class implementing IBankIdEndUserDeviceDataResolver");
        }

        Resolvers.Add(typeof(T));
    }

    public Type? ResolverFactory { get; private set; }

    public List<Type> Resolvers { get; } = new();

    public IServiceCollection Build()
    {
        return services;
    }
}
