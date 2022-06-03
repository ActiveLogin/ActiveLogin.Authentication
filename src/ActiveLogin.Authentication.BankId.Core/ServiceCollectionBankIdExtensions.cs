using System.Net.Http.Headers;
using System.Reflection;

using ActiveLogin.Authentication.BankId.Api.UserMessage;
using ActiveLogin.Authentication.BankId.Core.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.Core.Flow;
using ActiveLogin.Authentication.BankId.Core.Qr;
using ActiveLogin.Authentication.BankId.Core.ResultStore;
using ActiveLogin.Authentication.BankId.Core.SupportedDevice;
using ActiveLogin.Authentication.BankId.Core.UserData;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ActiveLogin.Authentication.BankId.Core;

public static class ServiceCollectionBankIdExtensions
{
    /// <summary>
    /// Add BankID authentication provider from Active Login.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="bankId">BankID configuration.</param>
    /// <example>
    /// <code>
    /// .AddBankId(bankId =>
    /// {
    ///     bankId
    ///         .UseProductionEnvironment()
    ///         .UseClientCertificateFromAzureKeyVault(configuration.GetSection("ActiveLogin:BankId:ClientCertificate"))
    ///         .UseQrCoderQrCodeGenerator();
    /// });
    /// </code>
    /// </example>
    /// <returns></returns>
    public static IServiceCollection AddBankId(this IServiceCollection services, Action<IBankIdBuilder> bankId)
    {
        var (activeLoginName, activeLoginVersion) = GetActiveLoginInfo();
        services.Configure<BankIdActiveLoginContext>(context =>
        {
            context.ActiveLoginProductName = activeLoginName;
            context.ActiveLoginProductVersion = activeLoginVersion;
        });

        var bankIdBuilder = new BankIdBuilder(services);

        AddBankIdDefaultServices(bankIdBuilder);
        UseUserAgentFromContext(bankIdBuilder);

        bankId(bankIdBuilder);

        return services;
    }

    private static void AddBankIdDefaultServices(IBankIdBuilder builder)
    {
        var services = builder.Services;

        services.AddTransient<IBankIdFlowSystemClock, BankIdFlowSystemClock>();
        services.AddTransient<IBankIdFlowService, BankIdFlowService>();

        services.AddTransient(x => x.GetRequiredService<IOptions<BankIdActiveLoginContext>>().Value);
        services.AddTransient<IBankIdEventTrigger, BankIdEventTrigger>();
        services.AddTransient<IBankIdUserMessage, BankIdRecommendedUserMessage>();
        services.AddTransient<IBankIdQrCodeGenerator, BankIdMissingQrCodeGenerator>();
        services.AddTransient<IBankIdSupportedDeviceDetectorByUserAgent, BankIdSupportedDeviceDetectorByUserAgent>();

        builder.UseAuthRequestUserDataResolver<BankIdAuthRequestEmptyUserDataResolver>();

        builder.AddEventListener<BankIdLoggerEventListener>();
        builder.AddEventListener<BankIdResultStoreEventListener>();

        builder.AddResultStore<BankIdResultTraceLoggerStore>();
    }

    private static void UseUserAgentFromContext(this IBankIdBuilder builder)
    {
        builder.ConfigureHttpClient((sp, httpClient) =>
        {
            var context = sp.GetRequiredService<BankIdActiveLoginContext>();
            var productInfoHeaderValue = new ProductInfoHeaderValue(context.ActiveLoginProductName, context.ActiveLoginProductVersion);

            httpClient.DefaultRequestHeaders.UserAgent.Clear();
            httpClient.DefaultRequestHeaders.UserAgent.Add(productInfoHeaderValue);
        });
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
