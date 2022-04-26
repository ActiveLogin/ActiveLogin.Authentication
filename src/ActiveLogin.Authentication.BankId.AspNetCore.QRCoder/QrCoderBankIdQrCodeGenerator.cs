using System;
using ActiveLogin.Authentication.BankId.AspNetCore.Qr;
using QRCoder;

namespace ActiveLogin.Authentication.BankId.AspNetCore.QrCoder
{
    /// <summary>
    /// This class generates QR codes for BankID
    /// </summary>
    /// <remarks>
    /// It uses the QR Coder library to generate the codes
    /// </remarks>
    public class QrCoderBankIdQrCodeGenerator : IBankIdQrCodeGenerator
    {
        private const int PixelsPerModule = 20;

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
            var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);

            using var qrCode = new PngByteQRCode(qrCodeData);
            var pngQrCode = qrCode.GetGraphic(PixelsPerModule);
            var base64QrCpde = Convert.ToBase64String(pngQrCode);

            return base64QrCpde;
        }
    }
}
