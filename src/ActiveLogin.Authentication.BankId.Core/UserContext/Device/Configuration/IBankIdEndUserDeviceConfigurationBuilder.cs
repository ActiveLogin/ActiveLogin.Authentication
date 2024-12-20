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
    void UseResolverFactory<T>() where T : class, IBankIdEndUserDeviceDataResolverFactory;

    /// <summary>
    /// Add a custom resolver for resolving the device data for a specific device type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    void UseDeviceResolver<T>() where T : class, IBankIdEndUserDeviceDataResolver;

    /// <summary>
    /// Add a custom resolver for resolving the device data for a specific device type,
    /// using a factory method.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="factory"></param>
    void UseDeviceResolver<T>(Func<IServiceProvider, T> factory) where T : class, IBankIdEndUserDeviceDataResolver;
    
}
