using ActiveLogin.Authentication.BankId.Api;

namespace ActiveLogin.Authentication.BankId.Core.UserContext.Device.Resolvers;

/// <inheritdoc cref="IBankIdEndUserDeviceDataResolver"/>
public abstract class BankIdDeviceDataResolverBase : IBankIdEndUserDeviceDataResolver
{
    public abstract BankIdEndUserDeviceType DeviceType { get; }

    public abstract Task<IBankIdEndUserDeviceData> GetDeviceDataAsync();

    public abstract IBankIdEndUserDeviceData GetDeviceData();


}
