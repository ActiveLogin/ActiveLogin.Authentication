namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    public enum EventSeverity
    {
        /// <summary>
        /// Success event
        /// </summary>
        Success = 1,

        /// <summary>
        /// Failure event
        /// </summary>
        Failure = 2,

        /// <summary>
        /// Information event
        /// </summary>
        Information = 3,

        /// <summary>
        /// Error event
        /// </summary>
        Error = 4
    }
}
