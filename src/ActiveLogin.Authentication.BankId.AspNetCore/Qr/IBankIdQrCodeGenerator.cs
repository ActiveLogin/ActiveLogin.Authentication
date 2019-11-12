namespace ActiveLogin.Authentication.BankId.AspNetCore.Qr
{
    /// <summary>
    /// This interface is used to generate QR codes for BankID
    /// </summary>
    public interface IBankIdQrCodeGenerator
    {
        string GenerateQrCodeAsBase64(string autoStartToken);
    }
}
