using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Qr
{
    public static class BankIdAuthenticationBuilderQrCodeExtensions
    {
        public static IBankIdAuthenticationBuilder UseDefaultQrCodeGenerator(this IBankIdAuthenticationBuilder builder)
        {
            var services = builder.AuthenticationBuilder.Services;

            services.TryAddTransient<IBankIdQrCodeGenerator, BankIdQrCoderGenerator>();

            return builder;
        }
    }
}
