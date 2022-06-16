using ActiveLogin.Authentication.BankId.Core.Qr;

using QRCoder;

/// <summary>
/// This class generates QR codes for BankID
/// </summary>
/// <remarks>
/// It uses the QR Coder library to generate the codes
/// </remarks>
public class QrCoderAsciiBankIdQrCodeGenerator : IBankIdQrCodeGenerator
{
    private const int QrPixelsPerModule = 1;
    private const QRCodeGenerator.ECCLevel QrEccLevel = QRCodeGenerator.ECCLevel.L;

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

        using var asciiQrCode = new AsciiQRCode(qrCodeData);
        var asciiQrCodeText = asciiQrCode.GetGraphic(
            QrPixelsPerModule,
            drawQuietZones: true,
            darkColorString: "*",
            whiteSpaceString: " " //██
        );
        var asciiQrCodeBytes = System.Text.Encoding.UTF8.GetBytes(asciiQrCodeText);
        var base64QrCode = Convert.ToBase64String(asciiQrCodeBytes);

        return base64QrCode;
    }
}
