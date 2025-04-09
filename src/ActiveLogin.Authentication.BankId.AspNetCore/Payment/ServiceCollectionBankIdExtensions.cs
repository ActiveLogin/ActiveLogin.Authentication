using ActiveLogin.Authentication.BankId.AspNetCore.ApplicationFeatureProviders;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.Core;

using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Payment;

public static class ServiceCollectionBankIdPaymentExtensions
{
    /// <summary>
    /// Add BankID payment provider from Active Login.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="bankId">BankID configuration.</param>
    /// <example>
    /// <code>
    /// .AddBankIdPayment(builder =>
    /// {
    ///     builder
    ///         .AddSameDevice()
    ///         .AddOtherDevice();
    /// });
    /// </code>
    /// </example>
    /// <returns></returns>
    public static IServiceCollection AddBankIdPayment(this IServiceCollection services, Action<IBankIdPaymentBuilder> bankId)
    {
        var (activeLoginName, activeLoginVersion) = BankIdCommonConfiguration.GetActiveLoginInfo();
        services.Configure<BankIdActiveLoginContext>(context =>
        {
            context.ActiveLoginProductName = activeLoginName;
            context.ActiveLoginProductVersion = activeLoginVersion;
        });

        var bankIdPaymentConfigurationProvider = new BankIdPaymentConfigurationProvider();
        services.AddTransient<IBankIdPaymentConfigurationProvider>(s => bankIdPaymentConfigurationProvider);

        var bankIdBuilder = new BankIdPaymentBuilder(services, bankIdPaymentConfigurationProvider);

        AddBankIdAuthAspNetServices(services);
        AddBankIdAuthDefaultServices(bankIdBuilder);

        bankId(bankIdBuilder);

        return services;
    }

    private static void AddBankIdAuthAspNetServices(IServiceCollection services)
    {
        services.AddLocalization(options =>
        {
            options.ResourcesPath = BankIdConstants.LocalizationResourcesPath;
        });

        services.AddControllersWithViews()
                .ConfigureApplicationPartManager(apm =>
                {
                    apm.FeatureProviders.Add(new BankIdUiPaymentControllerFeatureProvider());
                    apm.FeatureProviders.Add(new BankIdUiPaymentApiControllerFeatureProvider());
                });

        services.AddHttpContextAccessor();
    }

    private static void AddBankIdAuthDefaultServices(IBankIdPaymentBuilder builder)
    {
        var services = builder.Services;

        BankIdCommonConfiguration.AddDefaultServices(services);

        services.AddTransient<IBankIdUiStateProtector, BankIdUiStateProtector>();
        services.AddTransient<IBankIdUiResultProtector, BankIdUiResultProtector>();

        services.AddTransient<IBankIdPaymentService, BankIdPaymentService>();
    }
}
