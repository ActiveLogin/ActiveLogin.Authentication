using System.Reflection;

using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.StateHandling;
using ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice;
using ActiveLogin.Authentication.BankId.AspNetCore.UserContext;
using ActiveLogin.Authentication.BankId.AspNetCore.UserMessage;
using ActiveLogin.Authentication.BankId.Core;
using ActiveLogin.Authentication.BankId.Core.SupportedDevice;
using ActiveLogin.Authentication.BankId.Core.UserContext;
using ActiveLogin.Authentication.BankId.Core.UserMessage;

using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.BankId.AspNetCore;

internal static class BankIdCommonConfiguration
{
    private const string UnknownProductVersion = "Unknown";

    public static void AddDefaultServices(IServiceCollection services)
    {
        services.AddTransient<IBankIdUiOrderRefProtector, BankIdUiOrderRefProtector>();
        services.AddTransient<IBankIdQrStartStateProtector, BankIdQrStartStateProtector>();
        services.AddTransient<IBankIdUiOptionsProtector, BankIdUiOptionsProtector>();

        services.AddTransient<IBankIdInvalidStateHandler, BankIdCancelUrlInvalidStateHandler>();

        services.AddTransient<IBankIdSupportedDeviceDetector, BankIdSupportedDeviceDetector>();

        services.AddTransient<IBankIdUserMessageLocalizer, BankIdUserMessageStringLocalizer>();
        services.AddTransient<IBankIdEndUserIpResolver, BankIdRemoteIpAddressEndUserIpResolver>();
    }

    public static (string name, string version) GetActiveLoginInfo()
    {
        var productName = BankIdConstants.ProductName;
        var productAssembly = typeof(ServiceCollectionBankIdExtensions).Assembly;
        var assemblyFileVersion = productAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
        var productVersion = assemblyFileVersion?.Version ?? UnknownProductVersion;

        return (productName, productVersion);
    }
}
