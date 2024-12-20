using System;
using System.Threading.Tasks;

using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.Core.UserContext.Device;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Test.UserContext.Device.Resolvers;

public class FakeResolverApp : IBankIdEndUserDeviceDataResolver
{
    public BankIdEndUserDeviceType DeviceType { get; } = BankIdEndUserDeviceType.App;
    public Task<IBankIdEndUserDeviceData> GetDeviceDataAsync()
    {
        throw new NotImplementedException();
    }

    public IBankIdEndUserDeviceData GetDeviceData()
    {
        throw new NotImplementedException();
    }
}

public class FakeResolverWeb : IBankIdEndUserDeviceDataResolver
{
    public BankIdEndUserDeviceType DeviceType { get; } = BankIdEndUserDeviceType.Web;
    public Task<IBankIdEndUserDeviceData> GetDeviceDataAsync()
    {
        throw new NotImplementedException();
    }

    public IBankIdEndUserDeviceData GetDeviceData()
    {
        throw new NotImplementedException();
    }
}
