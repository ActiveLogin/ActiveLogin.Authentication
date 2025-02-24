using System.Reflection;

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
        services.AddTransient<IBankIdInvalidStateHandler, BankIdCancelUrlInvalidStateHandler>();

        services.AddTransient<IBankIdSupportedDeviceDetector, BankIdSupportedDeviceDetector>();

        services.AddTransient<IBankIdEndUserIpResolver, BankIdRemoteIpAddressEndUserIpResolver>();

        services.AddSingleton<IStateStorage, CookieStateStorage>();

        services.AddTransient<IBankIdDataStateProtector<Auth.BankIdUiAuthState>, BankIdUiAuthProtector>();
        services.AddTransient<BankIdDataStateProtector<Auth.BankIdUiAuthState>, BankIdUiAuthProtector>();

        services.AddTransient<IBankIdDataStateProtector<Sign.BankIdUiSignState>, BankIdUiSignProtector>();
        services.AddTransient<BankIdDataStateProtector<Sign.BankIdUiSignState>, BankIdUiSignProtector>();

        services.AddTransient<IBankIdDataStateProtector<Payment.BankIdUiPaymentState>, BankIdUiPaymentProtector>();
        services.AddTransient<BankIdDataStateProtector<Payment.BankIdUiPaymentState>, BankIdUiPaymentProtector>();

        services.AddTransient<IBankIdDataStateProtector<Models.BankIdUiOptions>, BankIdUiOptionsProtector>();
        services.AddTransient<BankIdDataStateProtector<Models.BankIdUiOptions>, BankIdUiOptionsProtector>();

        services.AddTransient<IBankIdDataStateProtector<Models.BankIdUiResult>, BankIdUiResultProtector>();
        services.AddTransient<BankIdDataStateProtector<Models.BankIdUiResult>, BankIdUiResultProtector>();

        services.AddTransient<IBankIdDataStateProtector<Core.Models.BankIdQrStartState>, BankIdQrStartStateProtector>();
        services.AddTransient<BankIdDataStateProtector<Core.Models.BankIdQrStartState>, BankIdQrStartStateProtector>();

        services.AddTransient<IBankIdDataStateProtector<Areas.ActiveLogin.Models.BankIdUiOrderRef>, BankIdUiOrderRefProtector>();
        services.AddTransient<BankIdDataStateProtector<Areas.ActiveLogin.Models.BankIdUiOrderRef>, BankIdUiOrderRefProtector>();

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
