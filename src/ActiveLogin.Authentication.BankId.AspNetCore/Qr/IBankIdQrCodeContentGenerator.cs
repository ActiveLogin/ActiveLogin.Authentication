namespace ActiveLogin.Authentication.BankId.AspNetCore.Qr;

public interface IBankIdQrCodeContentGenerator
{
    string Generate(string qrStartToken, string qrStartSecret, int time);
}
