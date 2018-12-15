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
        Unknown,

        // General

        /// <summary>
        /// The provided apiKey and AuthenticateServiceKey could not be validated: Authentication keys are wrong (FederatedLogin).
        /// </summary>
        ApiKeyNotValid01,

        /// <summary>
        /// Incorrect combination of authentication keys: Authentication keys are wrong (GetSession).
        /// </summary>
        FieldsNotValid,

        // BankID

        /// <summary>
        /// User canceled: Incorrect input data or error from BankID.
        /// </summary>
        Cancel1,

        /// <summary>
        /// User canceled: BankID is incorrectly installed or user aborted.
        /// </summary>
        Cancel2,

        /// <summary>
        /// Already in progress: Another session is currently trying to authenticate this personal number / certificate.
        /// Can happen if not all requests are completed before re-trying.
        /// </summary>
        Already_In_Progress,

        /// <summary>
        /// Error message was malformed: Error message was malformed.
        /// </summary>
        Unknown_Formatting,

        /// <summary>
        /// Transaction expired before user authenticated: The transaction was expired by bankid before it was completed on GrandIDs end.
        /// </summary>
        Expired_Transaction
    }
}