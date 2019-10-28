using QRCoder;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Models
{
    public class BankIdQrCodeGenerator : IBankIdQrCodeGenerator
    {
        public string GenerateQrCodeAsBase64(string autoStartToken)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();

            var qrUrl = $"bankid:///?autostarttoken={autoStartToken}";
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrUrl, QRCodeGenerator.ECCLevel.Q);

            Base64QRCode qrCode = new Base64QRCode(qrCodeData);

            return qrCode.GetGraphic(20);
        }
    }
}
