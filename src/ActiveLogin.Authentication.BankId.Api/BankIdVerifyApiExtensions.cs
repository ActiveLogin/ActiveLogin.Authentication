using ActiveLogin.Authentication.BankId.Api.Models;

namespace ActiveLogin.Authentication.BankId.Api;

public static class BankIdVerifyApiExtensions
{
    /// <summary>
    /// Perform verification of digital ID card from BankID.
    /// </summary>
    /// <param name="apiClient">The <see cref="IBankIdAppApiClient"/> instance.</param>
    /// <param name="qrCode">The complete content of the scanned QR code.</param>
    public static Task<VerifyResponse> VerifyAsync(this IBankIdVerifyApiClient apiClient, string qrCode)
    {
        return apiClient.VerifyAsync(new VerifyRequest(qrCode));
    }
}
