using System.Reflection;

using ActiveLogin.Authentication.BankId.AspNetCore;
using ActiveLogin.Authentication.BankId.AspNetCore.ApplicationFeatureProviders;
using ActiveLogin.Authentication.BankId.AspNetCore.ClaimsTransformation;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.EndUserContext;
using ActiveLogin.Authentication.BankId.AspNetCore.StateHandling;
using ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice;
using ActiveLogin.Authentication.BankId.AspNetCore.UserMessage;
using ActiveLogin.Authentication.BankId.Core;
using ActiveLogin.Authentication.BankId.Core.EndUserContext;
using ActiveLogin.Authentication.BankId.Core.StateHandling;
using ActiveLogin.Authentication.BankId.Core.SupportedDevice;
using ActiveLogin.Authentication.BankId.Core.UserMessage;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;
public static class AuthenticationBuilderBankIdExtensions
{
    /// <summary>
    /// Add BankID authentication provider from Active Login.
    /// </summary>
    /// <param name="authenticationBuilder"></param>
    /// <param name="bankId">BankID configuration.</param>
    /// <returns></returns>
    public static AuthenticationBuilder AddBankId(this AuthenticationBuilder authenticationBuilder, Action<IBankIdAuthBuilder> bankId)
    {
        var services = authenticationBuilder.Services;

        var (activeLoginName, activeLoginVersion) = GetActiveLoginInfo();
        services.Configure<BankIdActiveLoginContext>(context =>
        {
            context.ActiveLoginProductName = activeLoginName;
            context.ActiveLoginProductVersion = activeLoginVersion;
        });

        services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<BankIdOptions>, BankIdPostConfigureOptions>());

        var bankIdBuilder = new BankIdAuthBuilder(services, authenticationBuilder);

        AddBankIdAuthAspNetServices(bankIdBuilder.Services);
        AddBankIdAuthDefaultServices(bankIdBuilder);

        bankId(bankIdBuilder);

        return authenticationBuilder;
    }

    private static void AddBankIdAuthAspNetServices(IServiceCollection services)
    {
        services.AddControllersWithViews()
                .ConfigureApplicationPartManager(apm =>
                {
                    apm.FeatureProviders.Add(new BankIdControllerFeatureProvider());
                    apm.FeatureProviders.Add(new BankIdApiControllerFeatureProvider());
                });
        services.AddHttpContextAccessor();

        services.AddLocalization(options =>
        {
            options.ResourcesPath = BankIdConstants.LocalizationResourcesPath;
        });
    }

    private static void AddBankIdAuthDefaultServices(IBankIdAuthBuilder builder)
    {
        var services = builder.Services;

        services.AddTransient<IBankIdOrderRefProtector, BankIdOrderRefProtector>();
        services.AddTransient<IBankIdQrStartStateProtector, BankIdQrStartStateProtector>();
        services.AddTransient<IBankIdLoginOptionsProtector, BankIdLoginOptionsProtector>();
        services.AddTransient<IBankIdLoginResultProtector, BankIdLoginResultProtector>();

        services.AddTransient<IBankIdInvalidStateHandler, BankIdCancelUrlInvalidStateHandler>();

        services.AddTransient<IBankIdSupportedDeviceDetector, BankIdSupportedDeviceDetector>();

        services.AddTransient<IBankIdUserMessageLocalizer, BankIdUserMessageStringLocalizer>();
        services.AddTransient<IBankIdEndUserIpResolver, BankIdRemoteIpAddressEndUserIpResolver>();

        builder.AddClaimsTransformer<BankIdDefaultClaimsTransformer>();
    }

    private static (string name, string version) GetActiveLoginInfo()
    {
        var productName = BankIdConstants.ProductName;
        var productAssembly = typeof(ServiceCollectionBankIdExtensions).Assembly;
        var assemblyFileVersion = productAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
        var productVersion = assemblyFileVersion?.Version ?? "Unknown";

        return (productName, productVersion);
    }
}
