using ActiveLogin.Authentication.BankId.AspNetCore.Qr;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ActiveLogin.Authentication.BankId.AspNetCore.QrCoder
{
    public static class BankIdAuthenticationBuilderQrCoderExtensions
    {
        public static IBankIdAuthenticationBuilder UseQrCoderQrCodeGenerator(this IBankIdAuthenticationBuilder builder)
        {
            var services = builder.AuthenticationBuilder.Services;

            services.TryAddTransient<IBankIdQrCodeGenerator, BankIdQrCoderGenerator>();

            return builder;
        }
    }
}
