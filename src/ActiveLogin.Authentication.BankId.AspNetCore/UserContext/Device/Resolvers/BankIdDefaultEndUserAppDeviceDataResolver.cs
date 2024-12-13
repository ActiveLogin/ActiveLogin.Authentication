using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.Core.UserContext.Device;
using ActiveLogin.Authentication.BankId.Core.UserContext.Device.Exceptions;

namespace ActiveLogin.Authentication.BankId.AspNetCore.UserContext.Device.Resolvers;

/// <inheritdoc cref="IBankIdEndUserDeviceDataResolver"/>
public sealed class BankIdDefaultEndUserAppDeviceDataResolver : BankIdDefaultEndUserDeviceDataResolverBase
{
    public override BankIdEndUserDeviceType DeviceType => BankIdEndUserDeviceType.App;

    public override Task<IBankIdEndUserDeviceData> GetDeviceDataAsync()
    {
        return Task.FromResult(GetDeviceData());
    }

    public override IBankIdEndUserDeviceData GetDeviceData()
    {
        TryGetAppDeviceParameters(out var appDeviceParameters);
        return appDeviceParameters ?? throw new DeviceDataException("Could not resolve device parameters for web device");
    }

    private bool TryGetAppDeviceParameters(out DeviceDataApp? parameters)
    {
        parameters = null;

        return false;
    }

}
