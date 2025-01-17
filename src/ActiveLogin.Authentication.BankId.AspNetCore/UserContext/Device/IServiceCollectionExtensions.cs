using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.UserContext.Device.Resolvers;
using ActiveLogin.Authentication.BankId.Core.UserContext.Device;
using ActiveLogin.Authentication.BankId.Core.UserContext.Device.Configuration;
using ActiveLogin.Authentication.BankId.Core.UserContext.Device.ResolverFactory;

using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.UserContext.Device;
public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddDefaultDeviceData(this IServiceCollection services)
    {
        services.AddTransient<IBankIdEndUserDeviceDataResolverFactory, BankIdDefaultEndUserDeviceDataResolverFactory>();
        services.AddSingleton<IBankIdEndUserDeviceDataConfiguration>(new DefaultBankIdEndUserDeviceDataConfiguration());
        services.AddTransient<IBankIdEndUserDeviceDataResolver, BankIdDefaultEndUserWebDeviceDataResolver>();
        services.AddTransient<IBankIdDeviceDataProtector, BankIdDeviceDataProtector>();
        return services;
    }

}
