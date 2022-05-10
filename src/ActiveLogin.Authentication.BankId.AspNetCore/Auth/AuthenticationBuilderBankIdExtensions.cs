using ActiveLogin.Authentication.BankId.AspNetCore;
using ActiveLogin.Authentication.BankId.AspNetCore.ApplicationFeatureProviders;
using ActiveLogin.Authentication.BankId.AspNetCore.Auth;
using ActiveLogin.Authentication.BankId.AspNetCore.ClaimsTransformation;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.Core;

using Microsoft.AspNetCore.Authentication;

namespace Microsoft.Extensions.DependencyInjection;

public static class AuthenticationBuilderBankIdAuthExtensions
{
    /// <summary>
    /// Add BankID authentication provider from Active Login.
    /// </summary>
    /// <param name="authenticationBuilder"></param>
    /// <param name="bankId">BankID configuration.</param>
    /// <example>
    /// <code>
    /// .AddBankId(bankId =>
    /// {
    ///     bankId
    ///         .AddSameDevice()
    ///         .AddOtherDevice();
    /// });
    /// </code>
    /// </example>
    /// <returns></returns>
    public static AuthenticationBuilder AddBankId(this AuthenticationBuilder authenticationBuilder, Action<IBankIdAuthBuilder> bankId)
    {
        var services = authenticationBuilder.Services;

        var (activeLoginName, activeLoginVersion) = BankIdCommonConfiguration.GetActiveLoginInfo();
        services.Configure<BankIdActiveLoginContext>(context =>
        {
            context.ActiveLoginProductName = activeLoginName;
            context.ActiveLoginProductVersion = activeLoginVersion;
        });

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
                    apm.FeatureProviders.Add(new BankIdUiAuthControllerFeatureProvider());
                    apm.FeatureProviders.Add(new BankIdUiAuthApiControllerFeatureProvider());
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

        BankIdCommonConfiguration.AddDefaultServices(services);

        services.AddTransient<IBankIdUiAuthStateProtector, BankIdUiAuthStateProtector>();
        services.AddTransient<IBankIdUiAuthResultProtector, BankIdUiAuthResultProtector>();

        builder.AddClaimsTransformer<BankIdDefaultClaimsTransformer>();
    }
}
