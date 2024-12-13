using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.Core.UserContext.Device;

namespace ActiveLogin.Authentication.BankId.AspNetCore.UserContext.Device.Resolvers;

/// <inheritdoc cref="IBankIdEndUserDeviceDataResolver"/>
public abstract class BankIdDefaultEndUserDeviceDataResolverBase : IBankIdEndUserDeviceDataResolver
{
    public abstract BankIdEndUserDeviceType DeviceType { get; }

    public abstract Task<IBankIdEndUserDeviceData> GetDeviceDataAsync();

    public abstract IBankIdEndUserDeviceData GetDeviceData();


}
