using ActiveLogin.Authentication.BankId.Api.Models;

namespace ActiveLogin.Authentication.BankId.Api;

/// <summary>
/// HTTP based client for the BankID Verify API.
/// </summary>
public class BankIdVerifyApiClient : IBankIdVerifyApiClient
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Creates an instance of <see cref="BankIdAppApiClient"/> using the supplied <see cref="HttpClient"/> to talk HTTP.
    /// </summary>
    /// <param name="httpClient">The HttpClient to use.</param>
    public BankIdVerifyApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<VerifyResponse> VerifyAsync(VerifyRequest request)
    {
        return _httpClient.PostAsync<VerifyRequest, VerifyResponse>("verify", request);
    }
}
