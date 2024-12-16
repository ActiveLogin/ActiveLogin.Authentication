using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.BankId.Core.UserContext.Device.Configuration;

/// <inheritdoc cref="IBankIdEndUserDeviceConfigurationBuilder"/>
public class BankIdEndUserDeviceConfigurationBuilder : IBankIdEndUserDeviceConfigurationBuilder
{
    public BankIdEndUserDeviceType DeviceType { get; set; } = BankIdEndUserDeviceType.Web;

    public void UseResolverFactory<T>() where T : IBankIdEndUserDeviceDataResolverFactory
    {
        if (typeof(T).IsInterface)
        {
            throw new ArgumentException("T must be a class implementing IBankIdEndUserDeviceDataResolverFactory");
        }

        ResolverFactory = new ServiceDescriptor(typeof(IBankIdEndUserDeviceDataResolverFactory), typeof(T),
            ServiceLifetime.Singleton);
    }

    public void AddDeviceResolver<T>() where T : IBankIdEndUserDeviceDataResolver
    {
        if (typeof(T).IsInterface)
        {
            throw new ArgumentException("T must be a class implementing IBankIdEndUserDeviceDataResolver");
        }

        _resolvers.Add(new ServiceDescriptor(typeof(IBankIdEndUserDeviceDataResolver), typeof(T),
            ServiceLifetime.Scoped));
    }

    public void AddDeviceResolver<T>(Func<IServiceProvider, T> factory) where T : IBankIdEndUserDeviceDataResolver
    {
        _resolvers.Add(new ServiceDescriptor(
            serviceType: typeof(IBankIdEndUserDeviceDataResolver),
            factory: provider => factory(provider),
            lifetime: ServiceLifetime.Scoped));
    }

    public ServiceDescriptor? ResolverFactory { get; private set; }

    private readonly List<ServiceDescriptor> _resolvers = new();

    public IReadOnlyList<ServiceDescriptor> Resolvers => _resolvers.AsReadOnly();

}
