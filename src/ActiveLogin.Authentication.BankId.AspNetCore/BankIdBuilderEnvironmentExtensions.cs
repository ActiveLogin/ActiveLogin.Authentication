using System;
using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.AspNetCore;
using ActiveLogin.Authentication.BankId.AspNetCore.Launcher;
using ActiveLogin.Authentication.BankId.Core;
using ActiveLogin.Authentication.BankId.Core.Launcher;

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

public static class BankIdBuilderEnvironmentExtensions
{
    internal static IBankIdBuilder UseEnvironment(this IBankIdBuilder builder, Uri apiBaseUrl, string environment)
    {
        SetActiveLoginContext(builder.AuthenticationBuilder.Services, environment, BankIdConstants.BankIdApiVersion);

        builder.ConfigureHttpClient(httpClient =>
        {
            httpClient.BaseAddress = apiBaseUrl;
        });

        builder.AuthenticationBuilder.Services.TryAddTransient<IBankIdLauncher, BankIdLauncher>();

        if (builder is BankIdBuilder bankIdBuilder)
        {
            bankIdBuilder.EnableHttpBankIdApiClient();
        }

        return builder;
    }

    /// <summary>
    /// Use the BankID test environment (https://appapi2.test.bankid.com/rp/vX/)
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IBankIdBuilder UseTestEnvironment(this IBankIdBuilder builder)
    {
        return builder.UseEnvironment(BankIdUrls.TestApiBaseUrl, BankIdEnvironments.Test);
    }

    /// <summary>
    /// Use the BankID production environment (https://appapi2.bankid.com/rp/vX/)
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IBankIdBuilder UseProductionEnvironment(this IBankIdBuilder builder)
    {
        return builder.UseEnvironment(BankIdUrls.ProductionApiBaseUrl, BankIdEnvironments.Production);
    }

    /// <summary>
    /// Use simulated (in memory) environment. To be used for automated testing.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IBankIdBuilder UseSimulatedEnvironment(this IBankIdBuilder builder)
        => UseSimulatedEnvironment(builder, x => new BankIdSimulatedApiClient());

    /// <summary>
    /// Use simulated (in memory) environment. To be used for automated testing.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="givenName">Fake given name</param>
    /// <param name="surname">Fake surname</param>
    /// <returns></returns>
    public static IBankIdBuilder UseSimulatedEnvironment(this IBankIdBuilder builder, string givenName, string surname)
        => UseSimulatedEnvironment(builder, x => new BankIdSimulatedApiClient(givenName, surname));

    /// <summary>
    /// Use simulated (in memory) environment. To be used for automated testing.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="givenName">Fake given name</param>
    /// <param name="surname">Fake surname</param>
    /// <param name="personalIdentityNumber">Fake personal identity number</param>
    /// <returns></returns>
    public static IBankIdBuilder UseSimulatedEnvironment(this IBankIdBuilder builder, string givenName, string surname, string personalIdentityNumber)
        => UseSimulatedEnvironment(builder, x => new BankIdSimulatedApiClient(givenName, surname, personalIdentityNumber));

    private static IBankIdBuilder UseSimulatedEnvironment(this IBankIdBuilder builder, Func<IServiceProvider, IBankIdApiClient> bankIdDevelopmentApiClient)
    {
        SetActiveLoginContext(builder.AuthenticationBuilder.Services, BankIdEnvironments.Simulated, BankIdConstants.BankIdApiVersion);

        builder.AuthenticationBuilder.Services.AddSingleton(bankIdDevelopmentApiClient);
        builder.AuthenticationBuilder.Services.AddSingleton<IBankIdLauncher, BankIdDevelopmentLauncher>();

        return builder;
    }

    private static void SetActiveLoginContext(IServiceCollection services, string environment, string apiVersion)
    {
        services.Configure<BankIdActiveLoginContext>(context =>
        {
            context.BankIdApiEnvironment = environment;
            context.BankIdApiVersion = apiVersion;
        });
    }
}
