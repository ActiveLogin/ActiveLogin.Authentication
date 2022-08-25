using ActiveLogin.Authentication.BankId.Core;
using ActiveLogin.Authentication.BankId.Core.Qr;

using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.BankId.QrCoder;
public static class BankIdBuilderQrCoderExtensions
{
    /// <summary>
    /// Use QrCoder as the QR Code generator library.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IBankIdBuilder UseQrCoderQrCodeGenerator(this IBankIdBuilder builder)
    {
        var services = builder.Services;

        services.AddTransient<IBankIdQrCodeGenerator, QrCoderBankIdQrCodeGenerator>();

        return builder;
    }
}
