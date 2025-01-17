using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ActiveLogin.Authentication.BankId.Core.UserContext.Device.Configuration;

/// <inheritdoc cref="IBankIdEndUserDeviceConfigurationBuilder"/>
public class BankIdEndUserDeviceConfigurationBuilder(IBankIdBuilder bankIdBuilder) : IBankIdEndUserDeviceConfigurationBuilder
{
    public BankIdEndUserDeviceType DeviceType { get; set; } = BankIdEndUserDeviceType.Web;

    public void UseResolverFactory<T>() where T : class, IBankIdEndUserDeviceDataResolverFactory
    {
        if (typeof(T).IsInterface)
        {
            throw new ArgumentException("T must be a class implementing IBankIdEndUserDeviceDataResolverFactory");
        }

        bankIdBuilder.Services.Replace(new ServiceDescriptor(typeof(IBankIdEndUserDeviceDataResolverFactory), typeof(T),
            ServiceLifetime.Scoped));
    }

    public void UseDeviceResolver<T>() where T : class, IBankIdEndUserDeviceDataResolver
    {
        if (typeof(T).IsInterface)
        {
            throw new ArgumentException("T must be a class implementing IBankIdEndUserDeviceDataResolver");
        }

        bankIdBuilder.Services.Replace(
            new ServiceDescriptor(
                typeof(IBankIdEndUserDeviceDataResolver),
                typeof(T),
                ServiceLifetime.Scoped));
    }

    public void UseDeviceResolver<T>(Func<IServiceProvider, T> factory) where T : class, IBankIdEndUserDeviceDataResolver
    {
        if (typeof(T).IsInterface)
        {
            throw new ArgumentException("T must be a class implementing IBankIdEndUserDeviceDataResolver");
        }

        bankIdBuilder.Services.Replace(
            new ServiceDescriptor(
                typeof(IBankIdEndUserDeviceDataResolver),
                factory,
                ServiceLifetime.Scoped));
    }
    
}
