using System.Reflection;

namespace ActiveLogin.Authentication.BankId.Core.Qr;

public class BankIdMissingQrCodeGenerator : IBankIdQrCodeGenerator
{
    const string DefaultImageResourceIdentifier = "ActiveLogin.Authentication.BankId.Core.Qr.qr-default.png";

    public string GenerateQrCodeAsBase64(string content)
    {
        
        var assembly = typeof(BankIdMissingQrCodeGenerator).GetTypeInfo().Assembly;
        var resourceStream = assembly.GetManifestResourceStream(DefaultImageResourceIdentifier);

        if (resourceStream == null)
        {
            throw new Exception($"Can't find QR Code image: {DefaultImageResourceIdentifier}");
        }

        var base64EncodedImage = ConvertToBase64(resourceStream);

        return base64EncodedImage;
    }

    private static string ConvertToBase64(Stream stream)
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
