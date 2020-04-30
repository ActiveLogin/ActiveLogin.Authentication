namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    public enum EventSeverity
    {
        /// <summary>
        /// Information event.
        /// </summary>
        Information = 1,

        /// <summary>
        /// Success event.
        /// </summary>
        Success = 2,

        /// <summary>
        /// Failure event. Occurs when a known / handled scenario occurs that is not succeeded.
        /// </summary>
        Failure = 3,

        /// <summary>
        /// Error event. Occurs when an unknown / unhandled scenario occurs that is not succeeded.
        /// </summary>
        Error = 4
    }
}
