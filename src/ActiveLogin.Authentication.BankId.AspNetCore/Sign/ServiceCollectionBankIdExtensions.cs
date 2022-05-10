using System.Reflection;

using ActiveLogin.Authentication.BankId.AspNetCore;
using ActiveLogin.Authentication.BankId.AspNetCore.ApplicationFeatureProviders;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Sign;
using ActiveLogin.Authentication.BankId.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionBankIdSignExtensions
{
    /// <summary>
    /// Add BankID sign provider from Active Login.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="bankId">BankID configuration.</param>
    /// <example>
    /// <code>
    /// .AddBankIdSign(builder =>
    /// {
    ///     builder
    ///         .AddSameDevice()
    ///         .AddOtherDevice();
    /// });
    /// </code>
    /// </example>
    /// <returns></returns>
    public static IServiceCollection AddBankIdSign(this IServiceCollection services, Action<IBankIdSignBuilder> bankId)
    {
        var (activeLoginName, activeLoginVersion) = BankIdCommonConfiguration.GetActiveLoginInfo();
        services.Configure<BankIdActiveLoginContext>(context =>
        {
            context.ActiveLoginProductName = activeLoginName;
            context.ActiveLoginProductVersion = activeLoginVersion;
        });

        var bankIdSignConfigurationProvider = new BankIdSignConfigurationProvider();
        services.AddTransient<IBankIdSignConfigurationProvider>(s => bankIdSignConfigurationProvider);

        var bankIdBuilder = new BankIdSignBuilder(services, bankIdSignConfigurationProvider);

        AddBankIdAuthAspNetServices(services);
        AddBankIdAuthDefaultServices(bankIdBuilder);

        bankId(bankIdBuilder);

        return services;
    }

    private static void AddBankIdAuthAspNetServices(IServiceCollection services)
    {
        services.AddControllersWithViews()
                .ConfigureApplicationPartManager(apm =>
                {
                    apm.FeatureProviders.Add(new BankIdUiSignControllerFeatureProvider());
                    apm.FeatureProviders.Add(new BankIdUiSignApiControllerFeatureProvider());
                });
        services.AddHttpContextAccessor();

        services.AddLocalization(options =>
        {
            options.ResourcesPath = BankIdConstants.LocalizationResourcesPath;
        });
    }

    private static void AddBankIdAuthDefaultServices(IBankIdSignBuilder builder)
    {
        var services = builder.Services;

        BankIdCommonConfiguration.AddDefaultServices(services);

        services.AddTransient<IBankIdUiSignStateProtector, BankIdUiSignStateProtector>();
        //services.AddTransient<IBankIdUiSignResultProtector, BankIdUiSignResultProtector>();

        services.AddTransient<IBankIdSignService, BankIdSignService>();
    }
}
