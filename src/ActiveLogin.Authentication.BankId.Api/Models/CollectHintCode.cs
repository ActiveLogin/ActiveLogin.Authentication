namespace ActiveLogin.Authentication.BankId.Api.Models
{
    /// <summary>
    /// Possible values of hint code for collect call.
    /// </summary>
    public enum CollectHintCode
    {
        /// <summary>
        /// Unknown or new hint code.
        /// </summary>
        Unknown,

        // Pending

        /// <summary>
        /// The order is pending.
        /// The client has not yet received the order.
        /// The HintCode will later change to NoClient, Started or UserSign.
        /// </summary>
        OutstandingTransaction,

        /// <summary>
        /// The order is pending.
        /// The client has not yet received the order.
        /// </summary>
        NoClient,

        /// <summary>
        /// The order is pending.
        /// A client has been started with the autostarttoken but a usable ID has not yet been found in the started client.
        /// When the client starts there may be a short delay until all ID:s are registered.
        /// The user may not have any usable ID:s at all, or has not yet inserted their smart card.
        /// </summary>
        Started,

        /// <summary>
        /// The order is pending.
        /// The client has received the order.
        /// </summary>
        UserSign,

        // Failed

        /// <summary>
        /// The order has expired.
        /// The BankID security app/program did not start, the user did not finalize the signing or the RP called collect too late.
        /// </summary>
        ExpiredTransaction,

        /// <summary>
        /// This error is returned if:
        /// 1) The user has entered wrong security code too many times. The BankID cannot be used.
        /// 2) The users BankID is revoked.
        /// 3) The users BankID is invalid.
        /// </summary>
        CertificateErr,

        /// <summary>
        /// The user decided to cancel the order.
        /// </summary>
        UserCancel,

        /// <summary>
        /// The order was cancelled. The system received a new order for the user.
        /// </summary>
        Cancelled,

        /// <summary>
        /// The user did not provide her ID, or the RP requires AutoStartToken to be used, but the client did not start within a certain time limit. The reason may be:
        /// 1) RP did not use autoStartToken when starting BankID security program/app. RP must correct this in their implementation.
        /// 2) The client software was not installed or other problem with the user’s computer.
        /// </summary>
        StartFailed
    }
}