using System;
using System.Threading.Tasks;

using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.Core.UserContext.Device;

namespace ActiveLogin.Authentication.BankId.Core.Test.UserContext.Device;

public class FakeResolver : IBankIdEndUserDeviceDataResolver
{
    public BankIdEndUserDeviceType DeviceType { get; }
    public Task<IBankIdEndUserDeviceData> GetDeviceDataAsync()
    {
        throw new NotImplementedException();
    }

    public IBankIdEndUserDeviceData GetDeviceData()
    {
        throw new NotImplementedException();
    }
}
