using ActiveLogin.Authentication.BankId.AspNetCore;
using ActiveLogin.Authentication.BankId.AspNetCore.Qr;
using ActiveLogin.Authentication.BankId.AspNetCore.QrCoder;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class BankIdAuthenticationBuilderQrCoderExtensions
    {
        public static IBankIdAuthenticationBuilder UseQrCoderQrCodeGenerator(this IBankIdAuthenticationBuilder builder)
        {
            var services = builder.AuthenticationBuilder.Services;

            services.AddTransient<IBankIdQrCodeGenerator, QrCoderBankIdQrCodeGenerator>();

            return builder;
        }
    }
}
