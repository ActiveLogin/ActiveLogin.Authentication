using ActiveLogin.Authentication.BankId.Core.UserContext.Device.Exceptions;

namespace ActiveLogin.Authentication.BankId.Core.UserContext.Device.ResolverFactory;

public sealed class BankIdDefaultEndUserDeviceDataResolverFactory(
    IBankIdEndUserDeviceDataConfiguration deviceDataConfiguration,
    IEnumerable<IBankIdEndUserDeviceDataResolver> resolvers)
    : IBankIdEndUserDeviceDataResolverFactory
{
    public IBankIdEndUserDeviceDataResolver GetResolver()
    {
        return resolvers
                   .First(dataResolver =>
                       dataResolver.DeviceType == deviceDataConfiguration.DeviceType)
               ?? throw new BankIdDeviceDataResolverException(deviceDataConfiguration.DeviceType);
    }

}
