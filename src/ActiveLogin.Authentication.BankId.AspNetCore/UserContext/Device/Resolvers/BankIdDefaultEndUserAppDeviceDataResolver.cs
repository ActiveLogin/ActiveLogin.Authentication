using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.Core.UserContext.Device;
using ActiveLogin.Authentication.BankId.Core.UserContext.Device.Exceptions;

namespace ActiveLogin.Authentication.BankId.AspNetCore.UserContext.Device.Resolvers;

/// <inheritdoc cref="IBankIdEndUserDeviceDataResolver"/>
public sealed class BankIdDefaultEndUserAppDeviceDataResolver : BankIdDefaultEndUserDeviceDataResolverBase
{
    public string AppIdentifier { get; init; } = string.Empty;
    public string DeviceOs { get; init; } = string.Empty;
    public string DeviceModelName { get; init; } = string.Empty;
    public string DeviceIdentifier { get; init; } = string.Empty;


    public override BankIdEndUserDeviceType DeviceType => BankIdEndUserDeviceType.App;

    public override Task<IBankIdEndUserDeviceData> GetDeviceDataAsync()
    {
        return Task.FromResult(GetDeviceData());
    }

    public override IBankIdEndUserDeviceData GetDeviceData()
    {
        TryGetAppDeviceParameters(out var appDeviceParameters);
        return appDeviceParameters ?? throw new DeviceDataException("Could not resolve device parameters for app device");
    }

    private bool TryGetAppDeviceParameters(out DeviceDataApp? parameters)
    {
        parameters = new DeviceDataApp(AppIdentifier, DeviceOs, DeviceModelName, DeviceIdentifier);
        return true;
    }

}
