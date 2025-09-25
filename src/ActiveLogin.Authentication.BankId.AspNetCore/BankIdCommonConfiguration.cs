using System.Reflection;

using ActiveLogin.Authentication.BankId.AspNetCore.Cookies;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.StateHandling;
using ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice;
using ActiveLogin.Authentication.BankId.AspNetCore.UserContext;
using ActiveLogin.Authentication.BankId.AspNetCore.UserContext.Device;
using ActiveLogin.Authentication.BankId.Core;
using ActiveLogin.Authentication.BankId.Core.SupportedDevice;
using ActiveLogin.Authentication.BankId.Core.UserContext;

using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.BankId.AspNetCore;

internal static class BankIdCommonConfiguration
{
    private const string UnknownProductVersion = "Unknown";

    public static void AddDefaultServices(IServiceCollection services)
    {
        services.AddSingleton<IBankIdInvalidStateHandler, BankIdCancelUrlInvalidStateHandler>();
        services.AddSingleton<IBankIdSupportedDeviceDetector, BankIdSupportedDeviceDetector>();
        services.AddSingleton<IBankIdEndUserIpResolver, BankIdRemoteIpAddressEndUserIpResolver>();

        services.AddSingleton<IStateStorage, CookieStateStorage>();

        services.AddSingleton<IBankIdDataStateProtector<Auth.BankIdUiAuthState>, BankIdUiAuthProtector>();
        services.AddSingleton<IBankIdDataStateProtector<Sign.BankIdUiSignState>, BankIdUiSignProtector>();
        services.AddSingleton<IBankIdDataStateProtector<Payment.BankIdUiPaymentState>, BankIdUiPaymentProtector>();
        services.AddSingleton<IBankIdDataStateProtector<Models.BankIdUiOptions>, BankIdUiOptionsProtector>();
        services.AddSingleton<IBankIdDataStateProtector<Models.BankIdUiResult>, BankIdUiResultProtector>();
        services.AddSingleton<IBankIdDataStateProtector<Core.Models.BankIdQrStartState>, BankIdQrStartStateProtector>();
        services.AddSingleton<IBankIdDataStateProtector<Areas.ActiveLogin.Models.BankIdUiOrderRef>, BankIdUiOrderRefProtector>();

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
