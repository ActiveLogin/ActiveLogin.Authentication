using System;

using ActiveLogin.Authentication.BankId.Core.UserContext.Device;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Test.UserContext.Device.Resolvers;

public class FakeResolverFactory : IBankIdEndUserDeviceDataResolverFactory
{
    public IBankIdEndUserDeviceDataResolver GetResolver()
    {
        throw new NotImplementedException();
    }
}
