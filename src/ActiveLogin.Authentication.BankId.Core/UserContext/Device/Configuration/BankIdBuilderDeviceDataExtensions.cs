using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ActiveLogin.Authentication.BankId.Core.UserContext.Device.Configuration;
public static class BankIdBuilderDeviceDataExtensions
{
    /// <summary>
    /// Configure the end user device data for BankID.
    /// </summary>
    /// <param name="bankIdBuilder"></param>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IBankIdBuilder UseDeviceData(this IBankIdBuilder bankIdBuilder, Action<IBankIdEndUserDeviceConfigurationBuilder> builder)
    {
        var configBuilder = new BankIdEndUserDeviceConfigurationBuilder();
        builder(configBuilder);

        // Create the DeviceDataConfiguration and add it to the services
        bankIdBuilder.Services.Replace(new ServiceDescriptor(
            serviceType: typeof(IBankIdEndUserDeviceDataConfiguration),
            factory: _ => new DefaultBankIdEndUserDeviceDataConfiguration()
            {
                DeviceType = configBuilder.DeviceType
            },
            lifetime: ServiceLifetime.Singleton));

        // ResolverFactory set, remove default resolver factory and add custom resolver factory
        if (configBuilder.ResolverFactory != null)
        {
            bankIdBuilder.Services.Replace(configBuilder.ResolverFactory);
        }

        // Resolvers set, remove default resolvers and add custom resolvers
        if (configBuilder.Resolvers.Count > 0)
        {
            bankIdBuilder.Services.RemoveAll<IBankIdEndUserDeviceDataResolver>();
            foreach (var resolver in configBuilder.Resolvers)
            {
                bankIdBuilder.Services.Add(resolver);
            }
        }

        return bankIdBuilder;
    }
}
