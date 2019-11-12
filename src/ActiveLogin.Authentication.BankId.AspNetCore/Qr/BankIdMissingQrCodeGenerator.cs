using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Localization;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Qr
{
    internal class BankIdMissingQrCodeGenerator : IBankIdQrCodeGenerator
    {
        private readonly IStringLocalizer<BankIdAuthenticationHandler> _localizer;

        public BankIdMissingQrCodeGenerator(IStringLocalizer<BankIdAuthenticationHandler> localizer)
        {
            _localizer = localizer;
        }

        public string GenerateQrCodeAsBase64(string autoStartToken)
        {
            var fileName = _localizer["Qr_Code_Default_Image"];
            var assembly = typeof(BankIdMissingQrCodeGenerator).GetTypeInfo().Assembly;
            var resourceStream = assembly.GetManifestResourceStream($"ActiveLogin.Authentication.BankId.AspNetCore.Resources.{ fileName }");
            var base64EncodedImage = ConvertToBase64(resourceStream);

            return base64EncodedImage;
        }

        private string ConvertToBase64(Stream stream)
        {
            byte[] bytes;

            using (var memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }

            return Convert.ToBase64String(bytes);
        }
    }
}
