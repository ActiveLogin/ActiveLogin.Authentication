namespace ActiveLogin.Authentication.BankId.Api.Models;

/// <summary>
/// Possible values of error codes returned from an error response.
/// </summary>
public enum ErrorCode
{
    Unknown,

    // 400
    /// <summary>
    /// An auth or sign request with personal number was sent, but an order for the user is already in progress.
    /// The order is aborted. No order is created.
    ///
    /// RP must inform the user that an auth or sign order is already in progress for the user. Message RFA4 should be used.
    /// </summary>
    AlreadyInProgress,

    /// <summary>
    /// Invalid parameter. Invalid use of method.
    /// * Using an orderRef that previously resulted in completed. The order cannot be collected twice.
    /// * Using an orderRef that previously resulted in failed. The order cannot be collected twice.
    /// * Using an orderRef that is too old. completed orders can only be collected up to 3 minutes and failed orders up to 5 minutes.
    /// * Cancelling an orderRef that is already cancelled. The order cannot be cancelled twice.
    ///
    /// RP must not try the same request again.
    /// This is an internal error within RP's system and must not be communicated to the user as a BankID error.
    /// </summary>
    InvalidParameters,

    Canceled,

    /// <summary>
    /// A call was processed, but resulted in a non-valid validation of the presented ID card.
    /// 
    /// Possible reasons:
    /// 
    /// The QR code did not correlate to a known ID card
    /// The ID card was closed before the validation request could be completed
    /// The ID card was revoked or expired
    /// </summary>
    VerificationFailed,

    // 401
    Unauthorized,

    // 404
    NotFound,

    // 405
    MethodNotAllowed,

    // 408,
    RequestTimeout,

    // 415,
    UnsupportedMediaType,

    // 429
    /// <summary>
    /// If the RP has called the APIs an excessive number of times.
    /// The response contains the following HTTP headers according to IETF draft-polli-ratelimit-headers-00 section 3:
    /// 
    /// RateLimit-Limit
    /// RateLimit-Remaining
    /// RateLimit-Reset
    /// 
    /// SeeRate-limiting under Technical details in section Introduction for more details.
    /// </summary>
    TooManyRequests,

    // 500,
    InternalError,

    // 503
    Maintenance
}
