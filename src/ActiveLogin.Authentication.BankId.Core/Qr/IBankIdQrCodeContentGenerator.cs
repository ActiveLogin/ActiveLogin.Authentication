namespace ActiveLogin.Authentication.BankId.Core.Qr;

public interface IBankIdQrCodeContentGenerator
{
    string Generate(string qrStartToken, string qrStartSecret, int time);
}
