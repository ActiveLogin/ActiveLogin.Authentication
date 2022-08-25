namespace ActiveLogin.Authentication.BankId.Core.Events.Infrastructure;

/// <summary>
/// BankId event severity.
/// </summary>
public enum BankIdEventSeverity
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
    /// Failure event.
    /// </summary>
    Failure = 3,

    /// <summary>
    /// Error event.
    /// </summary>
    Error = 4
}
