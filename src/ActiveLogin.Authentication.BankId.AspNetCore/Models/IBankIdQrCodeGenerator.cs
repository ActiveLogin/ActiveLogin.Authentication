namespace ActiveLogin.Authentication.BankId.AspNetCore.Models
{
    public interface IBankIdQrCodeGenerator
    {
        string GenerateQrCodeAsBase64(string autoStartToken);
    }
}
