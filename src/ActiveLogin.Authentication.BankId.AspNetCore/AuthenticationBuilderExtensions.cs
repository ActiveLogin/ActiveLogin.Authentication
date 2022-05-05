using System.Reflection;
using ActiveLogin.Authentication.BankId.AspNetCore;
using ActiveLogin.Authentication.BankId.Core;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;

public static class AuthenticationBuilderExtensions
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

        bankIdBuilder.AddDefaultServices();

        bankId(bankIdBuilder);

        return authenticationBuilder;
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
