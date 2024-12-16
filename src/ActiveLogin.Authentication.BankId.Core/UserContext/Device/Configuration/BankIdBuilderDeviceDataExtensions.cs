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
        var configBuilder = new BankIdEndUserDeviceConfigurationBuilder(bankIdBuilder);
        builder(configBuilder);

        // Create the DeviceDataConfiguration and add it to the services
        bankIdBuilder.Services.Replace(new ServiceDescriptor(
            serviceType: typeof(IBankIdEndUserDeviceDataConfiguration),
            factory: _ => new DefaultBankIdEndUserDeviceDataConfiguration()
            {
                DeviceType = configBuilder.DeviceType
            },
            lifetime: ServiceLifetime.Singleton));
        
        return bankIdBuilder;
    }
}
