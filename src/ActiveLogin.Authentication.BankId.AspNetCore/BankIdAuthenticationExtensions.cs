using System;
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
            var descriptor = ServiceDescriptor.Singleton<IPostConfigureOptions<BankIdAuthenticationOptions>, BankIdAuthenticationPostConfigureOptions>();
            builder.Services.TryAddEnumerable(descriptor);

            var bankIdAuthenticationBuilder = new BankIdAuthenticationBuilder(builder);
            bankIdAuthenticationBuilder.AddDefaultServices();
            bankId(bankIdAuthenticationBuilder);

            return builder;
        }
    }
}
