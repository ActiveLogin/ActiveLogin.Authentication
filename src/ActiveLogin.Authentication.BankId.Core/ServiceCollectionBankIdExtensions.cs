using System.Net.Http.Headers;
using System.Reflection;

using ActiveLogin.Authentication.BankId.Api.UserMessage;
using ActiveLogin.Authentication.BankId.Core;
using ActiveLogin.Authentication.BankId.Core.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.Core.Flow;
using ActiveLogin.Authentication.BankId.Core.Persistence;
using ActiveLogin.Authentication.BankId.Core.Qr;
using ActiveLogin.Authentication.BankId.Core.StateHandling;
using ActiveLogin.Authentication.BankId.Core.SupportedDevice;
using ActiveLogin.Authentication.BankId.Core.UserData;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionBankIdExtensions
{
    /// <summary>
    /// Add BankID authentication provider from Active Login.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="bankId">BankID configuration.</param>
    /// <example>
    /// <code>
    /// .AddBankId(builder =>
    /// {
    ///     builder
    ///         .UseProductionEnvironment()
    ///         .UseClientCertificateFromAzureKeyVault(configuration.GetSection("ActiveLogin:BankId:ClientCertificate"))
    ///         .AddSameDevice()
    ///         .AddOtherDevice()
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
        UseUserAgent(bankIdBuilder, new ProductInfoHeaderValue(activeLoginName, activeLoginVersion));

        bankId(bankIdBuilder);

        return services;
    }

    private static void AddBankIdDefaultServices(IBankIdBuilder builder)
    {
        var services = builder.Services;

        services.AddTransient<IBankIdFlowSystemClock, BankIdFlowSystemClock>();
        services.AddTransient<IBankIdFlowService, BankIdFlowService>();

        services.AddTransient<IBankIdEventTrigger, BankIdEventTrigger>();
        services.AddTransient<IBankIdUserMessage, BankIdRecommendedUserMessage>();
        services.AddTransient<IBankIdInvalidStateHandler, BankIdInvalidStateHandlerNoop>();
        services.AddTransient<IBankIdQrCodeGenerator, BankIdMissingQrCodeGenerator>();
        services.AddTransient<IBankIdSupportedDeviceDetectorByUserAgent, BankIdSupportedDeviceDetectorByUserAgent>();

        builder.UseAuthRequestUserDataResolver<BankIdAuthRequestEmptyUserDataResolver>();

        builder.AddEventListener<BankIdLoggerEventListener>();
        builder.AddEventListener<BankIdResultStoreEventListener>();

        builder.AddResultStore<BankIdResultTraceLoggerStore>();
    }

    private static void UseUserAgent(this IBankIdBuilder builder, ProductInfoHeaderValue productInfoHeaderValue)
    {
        builder.ConfigureHttpClient(httpClient =>
        {
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
