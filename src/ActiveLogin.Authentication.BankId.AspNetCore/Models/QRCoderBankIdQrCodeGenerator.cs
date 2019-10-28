using System.Collections.Generic;
using Microsoft.AspNetCore.Http.Extensions;
using QRCoder;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Models
{
    /// <summary>
    /// This class generates QR codes for BankID
    /// </summary>
    /// <remarks>
    /// It uses the QR Coder library to generate the codes
    /// </remarks>
    public class QRCoderBankIdQrCodeGenerator : IBankIdQrCodeGenerator
    {
        private const int PixelsPerModule = 20;

        /// <summary>
        /// Generates a QR code for BankID using the auto start token.
        /// </summary>
        /// <remarks>
        /// First the token is added to the BankID URL for auto start and
        /// then the QR code is generated from the resulting URL.
        /// </remarks>
        /// <param name="autoStartToken"></param>
        /// <returns>A base 64 representation of the QR code</returns>
        public string GenerateQrCodeAsBase64(string autoStartToken)
        {
            var queryPart = GetQueryPart(autoStartToken);
            var qrUrl = $"bankid:///{queryPart}";

            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(qrUrl, QRCodeGenerator.ECCLevel.Q);

            var qrCode = new Base64QRCode(qrCodeData);

            return qrCode.GetGraphic(PixelsPerModule);
        }

        private string GetQueryPart(string autoStartToken)
        {
            var queryStringParams = new Dictionary<string, string>();
            queryStringParams.Add("autostarttoken", autoStartToken);
            var queryBuilder = new QueryBuilder(queryStringParams);
            return queryBuilder.ToQueryString().ToString();
        }
    }
}
