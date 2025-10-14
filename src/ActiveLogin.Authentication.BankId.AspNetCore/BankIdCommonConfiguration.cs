using System.Reflection;

using ActiveLogin.Authentication.BankId.AspNetCore.Cookies;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Launcher;
using ActiveLogin.Authentication.BankId.AspNetCore.StateHandling;
using ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice;
using ActiveLogin.Authentication.BankId.AspNetCore.UserContext;
using ActiveLogin.Authentication.BankId.AspNetCore.UserContext.Device;
using ActiveLogin.Authentication.BankId.Core;
using ActiveLogin.Authentication.BankId.Core.Launcher;
using ActiveLogin.Authentication.BankId.Core.SupportedDevice;
using ActiveLogin.Authentication.BankId.Core.UserContext;

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

        services.AddTransient<IBankIdEndUserIpResolver, BankIdRemoteIpAddressEndUserIpResolver>();

        services.AddTransient<ICustomBrowserResolver, BankIdCustomBrowserResolver>();
        services.AddTransient<IBankIdRedirectUrlResolver, BankIdRedirectUrlResolver>();

        services.AddHttpContextAccessor();
        services.AddTransient<IBankIdUiOptionsCookieManager, BankIdUiOptionsCookieManager>();

        services.AddDefaultDeviceData();

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
