using ActiveLogin.Authentication.BankId.Api.Models;

namespace ActiveLogin.Authentication.BankId.Api;

/// <summary>
/// BankID Verify API Client.
/// </summary>
public interface IBankIdVerifyApiClient
{
    /// <summary>
    /// Perform verification of digital ID card from BankID.
    /// </summary>
    Task<VerifyResponse> VerifyAsync(VerifyRequest request);
}
