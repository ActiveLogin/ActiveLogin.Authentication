using System.Net.Http.Headers;
using System.Reflection;

using ActiveLogin.Authentication.BankId.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
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

        bankIdBuilder.AddDefaultServices();
        bankIdBuilder.UseUserAgent(GetActiveLoginUserAgent(activeLoginName, activeLoginVersion));

        bankId(bankIdBuilder);

        return services;
    }

    private static ProductInfoHeaderValue GetActiveLoginUserAgent(string name, string version)
    {
        return new ProductInfoHeaderValue(name, version);
    }

    private static (string name, string version) GetActiveLoginInfo()
    {
        var productName = BankIdConstants.ProductName;
        var productAssembly = typeof(ServiceCollectionExtensions).Assembly;
        var assemblyFileVersion = productAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
        var productVersion = assemblyFileVersion?.Version ?? "Unknown";

        return (productName, productVersion);
    }
}
