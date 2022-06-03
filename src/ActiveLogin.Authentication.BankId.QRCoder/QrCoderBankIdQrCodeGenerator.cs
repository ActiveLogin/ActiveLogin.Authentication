using ActiveLogin.Authentication.BankId.Core.Qr;

using QRCoder;

namespace ActiveLogin.Authentication.BankId.QrCoder;

/// <summary>
/// This class generates QR codes for BankID
/// </summary>
/// <remarks>
/// It uses the QR Coder library to generate the codes
/// </remarks>
public class QrCoderBankIdQrCodeGenerator : IBankIdQrCodeGenerator
{
    private const int QrPixelsPerModule = 20;
    private const QRCodeGenerator.ECCLevel QrEccLevel = QRCodeGenerator.ECCLevel.Q;

    /// <summary>
    /// Generates a QR code for BankID using the auto start token.
    /// </summary>
    /// <remarks>
    /// First the token is added to the BankID URL for auto start and
    /// then the QR code is generated from the resulting URL.
    /// </remarks>
    /// <param name="content"></param>
    /// <returns>A base 64 representation of the QR code</returns>
    public string GenerateQrCodeAsBase64(string content)
    {

        using var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(content, QrEccLevel);

        using var qrCode = new PngByteQRCode(qrCodeData);
        var pngQrCode = qrCode.GetGraphic(QrPixelsPerModule);
        var base64QrCode = Convert.ToBase64String(pngQrCode);

        return base64QrCode;
    }
}
