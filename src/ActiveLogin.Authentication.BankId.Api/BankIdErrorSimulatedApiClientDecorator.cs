using ActiveLogin.Authentication.BankId.Api.Models;

namespace ActiveLogin.Authentication.BankId.Api;

/// <summary>
/// This decorator simulates errors from the BankID API.
/// An error is thrown with a probability of <paramref name="errorRate"/>.
/// The error is one of the errors in <paramref name="errors"/>.
/// An error is thrown only once for each session.
/// The session is based on the <see cref="Response.OrderRef"></see> property.
/// </summary>
/// <param name="decorated">The decorated ApiClient <see cref="IBankIdAppApiClient"></see></param>
/// <param name="errorRate">Probability of error</param>
/// <param name="errors">Errors to use</param>
/// <param name="varyErrorTypes">If true, the error type will vary between requests</param>
public class BankIdErrorSimulatedApiClientDecorator(
    IBankIdAppApiClient decorated,
    double? errorRate = null,
    Dictionary<ErrorCode, string>? errors = null,
    bool? varyErrorTypes = null) : IBankIdAppApiClient
{
    private readonly Dictionary<ErrorCode, string> _errors = errors ?? new Dictionary<ErrorCode, string>()
    {
        { ErrorCode.AlreadyInProgress , "Already in progress"},
        { ErrorCode.Maintenance , "Maintenance"},
        { ErrorCode.RequestTimeout , "Request timeout"},
        { ErrorCode.InternalError , "Internal error"}
    };

    private static double? ValidateErrorRate(double? rate)
    {
        if (rate > 1 || rate < 0)
        {
            throw new ArgumentException("Error rate must be between 0 and 1.");
        }

        return rate;
    }

    private readonly double _throwErrorThreshold = ValidateErrorRate(errorRate) ?? 0.1;
    private readonly bool _varyErrorTypes = varyErrorTypes ?? false;
    private Error? _lastError;

    public Task<AuthResponse> AuthAsync(AuthRequest request)
    {
        return CallImplementation(x => x.AuthAsync(request));
    }

    public Task<SignResponse> SignAsync(SignRequest request)
    {
        return CallImplementation(x => x.SignAsync(request));
    }

    public Task<PaymentResponse> PaymentAsync(PaymentRequest request)
    {
        return CallImplementation(x => x.PaymentAsync(request));
    }

    public Task<PhoneAuthResponse> PhoneAuthAsync(PhoneAuthRequest request)
    {
        return CallImplementation(x => x.PhoneAuthAsync(request));
    }

    public Task<PhoneSignResponse> PhoneSignAsync(PhoneSignRequest request)
    {
        return CallImplementation(x => x.PhoneSignAsync(request));
    }

    public Task<CollectResponse> CollectAsync(CollectRequest request)
    {
        return CallImplementation(x => x.CollectAsync(request));
    }

    public Task<CancelResponse> CancelAsync(CancelRequest request)
    {
        return CallImplementation(x => x.CancelAsync(request));
    }

    private async Task<TResponse> CallImplementation<TResponse>(Func<IBankIdAppApiClient, Task<TResponse>> call)
    where TResponse : class
    {
        if (ShouldThrowError())
        {
            // If _lastError is null, get a random error
            _lastError ??= GetRandomError();

            // If varyErrorTypes is true, the error type will vary between requests
            if (_varyErrorTypes)
            {
                _lastError = GetRandomError();
            }

            throw new BankIdApiException(
                _lastError,
                new HttpRequestException("Unknown"));
        }

        var result = await call(decorated);

        return result;
    }

    private bool ShouldThrowError()
    {
        var r = new Random();
        var result = r.NextDouble() < _throwErrorThreshold;
        return result;
    }

    private Error GetRandomError()
    {
        var r = new Random();
        var error = _errors.ElementAt(r.Next(0, _errors.Count));
        return new Error(error.Key.ToString(), error.Value);
    }

}
