using System;
using System.Net.Http.Headers;
using System.Reflection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace ActiveLogin.Authentication.BankId.AspNetCore
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
            var productVersion = productAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version;

            return new ProductInfoHeaderValue(productName, productVersion);
        }
    }
}