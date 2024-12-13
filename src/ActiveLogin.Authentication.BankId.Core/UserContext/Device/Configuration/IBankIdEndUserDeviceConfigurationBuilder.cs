using ActiveLogin.Authentication.BankId.Core.UserContext.Device;

using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.BankId.Core.UserContext.Device.Configuration;

public interface IBankIdEndUserDeviceConfigurationBuilder
{
    /// <summary>
    /// Set the device type for BankID. Default is Web.
    /// </summary>
    BankIdEndUserDeviceType DeviceType { get; set; }

    /// <summary>
    /// Use a custom resolver factory to resolve which IBankIdEndUserDeviceDataResolver to use
    /// for the device types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    void UseResolverFactory<T>() where T : IBankIdEndUserDeviceDataResolverFactory;

    /// <summary>
    /// Add a custom resolver for resolving the device data for a specific device type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    void AddDeviceResolver<T>() where T : IBankIdEndUserDeviceDataResolver;

    /// <summary>
    /// The resolver factory to use for resolving the device data.
    /// </summary>
    Type? ResolverFactory { get; }

    /// <summary>
    /// The resolvers to use for resolving the device data.
    /// </summary>
    List<Type> Resolvers { get; }

}
