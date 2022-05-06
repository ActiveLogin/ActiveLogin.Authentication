namespace ActiveLogin.Authentication.BankId.AspNetCore.Areas.ActiveLogin.Models;

public class BankIdLoginApiQrCodeResponse
{
    internal BankIdLoginApiQrCodeResponse(string qrCodeAsBase64)
    {
        QrCodeAsBase64 = qrCodeAsBase64;
    }

    public string? QrCodeAsBase64 { get; set; }
}