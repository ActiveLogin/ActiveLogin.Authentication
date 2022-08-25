using System.Security.Cryptography;
using System.Text;

namespace ActiveLogin.Authentication.BankId.Core.Qr;

internal static class BankIdQrCodeContentGenerator
{
    private const string BankIdQrCodePrefix = "bankid";

    public static string Generate(string qrStartToken, string qrStartSecret, int time)
    {
        var qrAuthCode = GetQrAuthCode(qrStartSecret, time);
        return $"{BankIdQrCodePrefix}.{qrStartToken}.{TimeAsString(time)}.{qrAuthCode}";
    }

    private static string GetQrAuthCode(string qrStartSecret, int time)
    {
        var keyByteArray = Encoding.ASCII.GetBytes(qrStartSecret);
        using var hmac = new HMACSHA256(keyByteArray);

        var inputByteArray = Encoding.ASCII.GetBytes(TimeAsString(time));
        var hash = hmac.ComputeHash(inputByteArray);

        return Convert.ToHexString(hash).ToLower();
    }

    private static string TimeAsString(int time) => time.ToString("D");
}
