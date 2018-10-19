using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public static class BankIdAuthenticationExtensions
    {
        public static AuthenticationBuilder AddBankId(this AuthenticationBuilder builder)
        {
            return builder.AddBankId(bankId =>
            {
                bankId
                    .UseDevelopmentEnvironment()
                    .AddSameDevice(options => { })
                    .AddOtherDevice(options => { });
            });
        }

        public static AuthenticationBuilder AddBankId(this AuthenticationBuilder builder, Action<IBankIdAuthenticationBuilder> bankId)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<BankIdAuthenticationOptions>, BankIdAuthenticationPostConfigureOptions>());

            var bankIdAuthenticationBuilder = new BankIdAuthenticationBuilder(builder);
            bankIdAuthenticationBuilder.AddDefaultServices();
            bankId(bankIdAuthenticationBuilder);

            return builder;
        }
    }
}