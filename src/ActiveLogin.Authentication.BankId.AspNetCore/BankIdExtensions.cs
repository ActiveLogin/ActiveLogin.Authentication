using System.Net.Http.Headers;
using System.Reflection;
using ActiveLogin.Authentication.BankId.AspNetCore;
using ActiveLogin.Authentication.BankId.Core;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class BankIdExtensions
{
    /// <summary>
    /// Add BankID authentication provider from Active Login.
    /// </summary>
    /// <param name="builder"></param>
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
    public static AuthenticationBuilder AddBankId(this AuthenticationBuilder builder, Action<IBankIdBuilder> bankId)
    {
        var (activeLoginName, activeLoginVersion) = GetActiveLoginInfo();
        builder.Services.Configure<BankIdActiveLoginContext>(context =>
        {
            context.ActiveLoginProductName = activeLoginName;
            context.ActiveLoginProductVersion = activeLoginVersion;
        });

        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<BankIdOptions>, BankIdPostConfigureOptions>());

        var bankIdBuilder = new BankIdBuilder(builder);

        bankIdBuilder.AddDefaultServices();
        bankIdBuilder.UseUserAgent(GetActiveLoginUserAgent(activeLoginName, activeLoginVersion));

        bankId(bankIdBuilder);

        return builder;
    }

    private static ProductInfoHeaderValue GetActiveLoginUserAgent(string name, string version)
    {
        return new ProductInfoHeaderValue(name, version);
    }

    private static (string name, string version) GetActiveLoginInfo()
    {
        var productName = BankIdConstants.ProductName;
        var productAssembly = typeof(BankIdExtensions).Assembly;
        var assemblyFileVersion = productAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
        var productVersion = assemblyFileVersion?.Version ?? "Unknown";

        return (productName, productVersion);
    }
}
