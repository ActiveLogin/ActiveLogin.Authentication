namespace ActiveLogin.Authentication.BankId.AspNetCore.Qr
{
    /// <summary>
    /// This interface is used to generate QR codes for BankID.
    /// </summary>
    public interface IBankIdQrCodeGenerator
    {
        /// <summary>
        /// Generates a Base64 encoded QR code
        /// </summary>
        /// <param name="autoStartToken">The BankID autoStartToken to base the QR code on.</param>
        /// <returns>Base64 encoded QR code</returns>
        string GenerateQrCodeAsBase64(string autoStartToken);
    }
}
