using ActiveLogin.Authentication.BankId.Api.Models;

namespace ActiveLogin.Authentication.BankId.Api;

/// <summary>
/// HTTP based client for the BankID App API.
/// </summary>
public class BankIdAppApiClient : IBankIdAppApiClient
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Creates an instance of <see cref="BankIdAppApiClient"/> using the supplied <see cref="HttpClient"/> to talk HTTP.
    /// </summary>
    /// <param name="httpClient">The HttpClient to use.</param>
    public BankIdAppApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Initiates an authentication order. Use the collect method to query the status of the order.
    /// </summary>
    /// <returns>If the request is successful, the OrderRef and AutoStartToken is returned.</returns>
    public Task<AuthResponse> AuthAsync(AuthRequest request)
    {
        return _httpClient.PostAsync<AuthRequest, AuthResponse>("auth", request);
    }

    /// <summary>
    /// Initiates a signing order. Use the collect method to query the status of the order.
    /// </summary>
    /// <returns>If the request is successful, the OrderRef and AutoStartToken is returned.</returns>
    public Task<SignResponse> SignAsync(SignRequest request)
    {
        return _httpClient.PostAsync<SignRequest, SignResponse>("sign", request);
    }

    /// <summary>
    /// Initiates a payment order. Use the collect method to query the status of the order.
    /// </summary>
    /// <returns>If the request is successful, the OrderRef and AutoStartToken is returned.</returns>
    public Task<PaymentResponse> PaymentAsync(PaymentRequest request)
    {
        return _httpClient.PostAsync<PaymentRequest, PaymentResponse>("payment", request);
    }

    /// <summary>
    /// Initiates a phone authentication order. Use the collect method to query the status of the order.
    /// </summary>
    /// <returns>If the request is successful, the OrderRef is returned.</returns>
    public Task<PhoneAuthResponse> PhoneAuthAsync(PhoneAuthRequest request)
    {
        return _httpClient.PostAsync<PhoneAuthRequest, PhoneAuthResponse>("phone/auth", request);
    }

    /// <summary>
    /// Initiates a phone signing order. Use the collect method to query the status of the order.
    /// </summary>
    /// <returns>If the request is successful, the OrderRef is returned.</returns>
    public Task<PhoneSignResponse> PhoneSignAsync(PhoneSignRequest request)
    {
        return _httpClient.PostAsync<PhoneSignRequest, PhoneSignResponse>("phone/sign", request);
    }

    /// <summary>
    /// Collects the result of a sign or auth order using the OrderRef as reference.
    /// RP should keep on calling collect every two seconds as long as status indicates pending.
    /// RP must abort if status indicates failed.
    /// </summary>
    /// <returns>The user identity is returned when complete.</returns>
    public Task<CollectResponse> CollectAsync(CollectRequest request)
    {
        return _httpClient.PostAsync<CollectRequest, CollectResponse>("collect", request);
    }

    /// <summary>
    /// Cancels an ongoing sign or auth order.
    /// This is typically used if the user cancels the order in your service or app.
    /// </summary>
    public Task<CancelResponse> CancelAsync(CancelRequest request)
    {
        return _httpClient.PostAsync<CancelRequest, CancelResponse>("cancel", request);
    }
}
