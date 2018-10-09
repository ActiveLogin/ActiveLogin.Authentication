namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    /// <summary>
    /// Possible values of error codes returned from an error response.
    /// </summary>
    public enum ErrorCode
    {
        /// <summary>
        /// Unknown error.
        /// </summary>
        UNKNOWN,

        // General

        /// <summary>
        /// The provided apiKey and AuthenticateServiceKey could not be validated: Authentication keys are wrong (FederatedLogin).
        /// </summary>
        APIKEYNOTVALID01,

        /// <summary>
        /// Incorrect combination of authentication keys: Authentication keys are wrong (GetSession).
        /// </summary>
        FIELDSNOTVALID,

        // BankID

        /// <summary>
        /// User canceled: Incorrect input data or error from BankID.
        /// </summary>
        CANCEL1,

        /// <summary>
        /// User canceled: BankID is incorrectly installed or user aborted.
        /// </summary>
        CANCEL2,

        /// <summary>
        /// Already in progress: Another session is currently trying to authenticate this personal number / certificate.
        /// Can happen if not all requests are completed before re-trying.
        /// </summary>
        ALREADY_IN_PROGRESS,

        /// <summary>
        /// Error message was malformed: Error message was malformed.
        /// </summary>
        UNKNOWN_FORMATTING,

        /// <summary>
        /// Transaction expired before user authenticated: The transaction was expired by bankid before it was completed on GrandIDs end.
        /// </summary>
        EXPIRED_TRANSACTION
    }
}