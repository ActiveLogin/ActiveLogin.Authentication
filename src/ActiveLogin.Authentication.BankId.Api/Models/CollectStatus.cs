namespace ActiveLogin.Authentication.BankId.Api.Models
{
    /// <summary>
    /// Possible values of status for collect call.
    /// </summary>
    public enum CollectStatus
    {
        /// <summary>
        /// Unknown or new status.
        /// </summary>
        Unknown,

        /// <summary>
        /// The order is being processed. HintCode describes the status of the order.
        /// </summary>
        Pending,

        /// <summary>
        /// Something went wrong with the order. HintCode describes the error.
        /// </summary>
        Failed,

        /// <summary>
        /// The order is complete. CompletionData holds user information.
        /// </summary>
        Complete
    }
}