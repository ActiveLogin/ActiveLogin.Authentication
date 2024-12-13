using System;

using ActiveLogin.Authentication.BankId.Core.UserContext.Device;

namespace ActiveLogin.Authentication.BankId.Core.Test.UserContext.Device;

public class FakeResolverFactory : IBankIdEndUserDeviceDataResolverFactory
{
    public IBankIdEndUserDeviceDataResolver GetResolver()
    {
        throw new NotImplementedException();
    }
}
