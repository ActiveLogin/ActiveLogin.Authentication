using ActiveLogin.Authentication.BankId.Core;
using ActiveLogin.Authentication.BankId.Core.Qr;
using ActiveLogin.Authentication.BankId.QrCoder;

namespace Microsoft.Extensions.DependencyInjection;
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
