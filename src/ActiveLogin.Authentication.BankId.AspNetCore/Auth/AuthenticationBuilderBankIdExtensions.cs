using ActiveLogin.Authentication.BankId.AspNetCore.ApplicationFeatureProviders;
using ActiveLogin.Authentication.BankId.AspNetCore.ClaimsTransformation;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.Core;
using ActiveLogin.Authentication.BankId.Core.Requirements;
using ActiveLogin.Authentication.BankId.Core.UserData;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Auth;

public static class AuthenticationBuilderBankIdAuthExtensions
{
    /// <summary>
    /// Add BankID authentication provider from Active Login.
    /// </summary>
    /// <param name="authenticationBuilder"></param>
    /// <param name="bankId">BankID configuration.</param>
    /// <example>
    /// <code>
    /// .AddBankIdAuth(bankId =>
    /// {
    ///     bankId
    ///         .AddSameDevice()
    ///         .AddOtherDevice();
    /// });
    /// </code>
    /// </example>
    /// <returns></returns>
    public static AuthenticationBuilder AddBankIdAuth(this AuthenticationBuilder authenticationBuilder, Action<IBankIdAuthBuilder> bankId)
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
        services.AddLocalization(options =>
        {
            options.ResourcesPath = BankIdConstants.LocalizationResourcesPath;
        });

        services.AddControllersWithViews()
                .ConfigureApplicationPartManager(apm =>
                {
                    apm.FeatureProviders.Add(new BankIdUiAuthControllerFeatureProvider());
                    apm.FeatureProviders.Add(new BankIdUiAuthApiControllerFeatureProvider());
                });

        services.AddHttpContextAccessor();
    }

    private static void AddBankIdAuthDefaultServices(IBankIdAuthBuilder builder)
    {
        var services = builder.Services;

        BankIdCommonConfiguration.AddDefaultServices(services);

        services.AddTransient<IBankIdUiStateProtector, BankIdUiStateProtector>();
        services.AddTransient<IBankIdUiResultProtector, BankIdUiResultProtector>();

        builder.AddClaimsTransformer<BankIdDefaultClaimsTransformer>();

        builder.UseAuthRequestUserDataResolver<BankIdAuthRequestEmptyUserDataResolver>();
        builder.UseAuthRequestRequirementsResolver<BankIdAuthRequestEmptyRequirementsResolver>();
    }
}
