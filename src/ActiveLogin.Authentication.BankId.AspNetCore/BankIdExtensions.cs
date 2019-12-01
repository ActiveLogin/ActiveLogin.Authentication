using System;
using System.Net.Http.Headers;
using System.Reflection;
using ActiveLogin.Authentication.BankId.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BankIdExtensions
    {
        public static AuthenticationBuilder AddBankId(this AuthenticationBuilder builder, Action<IBankIdBuilder> bankId)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<BankIdOptions>, BankIdPostConfigureOptions>());

            var bankIdBuilder = new BankIdBuilder(builder);

            bankIdBuilder.AddDefaultServices();
            bankIdBuilder.UseUserAgent(GetActiveLoginUserAgent());

            bankId(bankIdBuilder);

            return builder;
        }

        private static ProductInfoHeaderValue GetActiveLoginUserAgent()
        {
            var productName = BankIdConstants.ProductName;
            var productAssembly = typeof(BankIdExtensions).Assembly;
            var assemblyFileVersion = productAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
            var productVersion = assemblyFileVersion?.Version ?? "Unknown";

            return new ProductInfoHeaderValue(productName, productVersion);
        }
    }
}