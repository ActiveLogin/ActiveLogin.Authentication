using System;
using System.Net.Http.Headers;
using System.Reflection;
using ActiveLogin.Authentication.BankId.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BankIdAuthenticationExtensions
    {
        public static AuthenticationBuilder AddBankId(this AuthenticationBuilder builder, Action<IBankIdAuthenticationBuilder> bankId)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<BankIdAuthenticationOptions>, BankIdAuthenticationPostConfigureOptions>());

            var bankIdAuthenticationBuilder = new BankIdAuthenticationBuilder(builder);

            bankIdAuthenticationBuilder.AddDefaultServices();
            bankIdAuthenticationBuilder.UseUserAgent(GetActiveLoginUserAgent());

            bankId(bankIdAuthenticationBuilder);

            return builder;
        }

        private static ProductInfoHeaderValue GetActiveLoginUserAgent()
        {
            var productName = BankIdAuthenticationConstants.ProductName;
            var productAssembly = typeof(BankIdAuthenticationExtensions).Assembly;
            var assemblyFileVersion = productAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
            var productVersion = assemblyFileVersion?.Version ?? "Unknown";

            return new ProductInfoHeaderValue(productName, productVersion);
        }
    }
}