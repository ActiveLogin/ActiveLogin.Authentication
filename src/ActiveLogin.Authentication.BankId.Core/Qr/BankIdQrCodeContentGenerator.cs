using System;
using System.Security.Cryptography;
using System.Text;

namespace ActiveLogin.Authentication.BankId.Core.Qr;

internal class BankIdQrCodeContentGenerator : IBankIdQrCodeContentGenerator
{
    public string Generate(string qrStartToken, string qrStartSecret, int time)
    {
        var qrAuthCode = GetQrAuthCode(qrStartSecret, time);
        return $"bankid.{qrStartToken}.{TimeAsString(time)}.{qrAuthCode}";
    }

    private string GetQrAuthCode(string qrStartSecret, int time)
    {
        var keyByteArray = Encoding.ASCII.GetBytes(qrStartSecret);
        using var hmac = new HMACSHA256(keyByteArray);

        var inputByteArray = Encoding.ASCII.GetBytes(TimeAsString(time));
        var hash = hmac.ComputeHash(inputByteArray);

        return Convert.ToHexString(hash).ToLower();
    }

    private string TimeAsString(int time) => time.ToString("D");
}
