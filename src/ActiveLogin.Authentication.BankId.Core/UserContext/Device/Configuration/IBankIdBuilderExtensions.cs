using System.Data;

using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.BankId.Core.UserContext.Device.Configuration;
public static class BankIdBuilderExtensions
{
    /// <summary>
    /// Configure the end user device data for BankID.
    /// </summary>
    /// <param name="bankIdBuilder"></param>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IBankIdBuilder UseDeviceData(this IBankIdBuilder bankIdBuilder, Action<IBankIdEndUserDeviceConfigurationBuilder> builder)
    {
        var configBuilder = new BankIdEndUserDeviceConfigurationBuilder(bankIdBuilder.Services);
        builder(configBuilder);

        // ResolverFactory set, remove default resolver factory and add custom resolver factory
        if (configBuilder.ResolverFactory != null)
        {
            bankIdBuilder.Services.RemoveServicesOfType<IBankIdEndUserDeviceDataResolverFactory>();
            bankIdBuilder.Services.AddTransient(typeof(IBankIdEndUserDeviceDataResolverFactory), configBuilder.ResolverFactory);
        }

        // Resolvers set, remove default resolvers and add custom resolvers
        if (configBuilder.Resolvers.Count > 0)
        {
            bankIdBuilder.Services.RemoveServicesOfType<IBankIdEndUserDeviceDataResolver>();
            configBuilder.Resolvers.ForEach(resolver => bankIdBuilder.Services.AddScoped(typeof(IBankIdEndUserDeviceDataResolver), resolver));
        }

        // Create the DeviceDataConfiguration and add it to the services
        bankIdBuilder.Services.RemoveServicesOfType<IBankIdEndUserDeviceDataConfiguration>();
        bankIdBuilder.Services.AddSingleton<IBankIdEndUserDeviceDataConfiguration>(new DefaultBankIdEndUserDeviceDataConfiguration() { DeviceType = configBuilder.DeviceType });

        return bankIdBuilder;
    }

    private static void RemoveServicesOfType<T>(this IServiceCollection services)
    {
        if (services.IsReadOnly)
        {
            throw new ReadOnlyException($"{nameof(services)} is read only");
        }

        if (services.Count == 0)
        {
            return;
        }

        var serviceDescriptors = services.Where(descriptor => descriptor.ServiceType == typeof(T)).ToList();
        foreach (var serviceDescriptor in serviceDescriptors)
        {
            services.Remove(serviceDescriptor);
        }
    }

}
