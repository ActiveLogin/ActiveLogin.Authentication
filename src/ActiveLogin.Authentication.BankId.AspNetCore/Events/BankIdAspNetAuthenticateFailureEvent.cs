namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    /// <summary>
    /// Event for when handling the ASP.NET authentication fails.
    /// </summary>
    public class BankIdAspNetAuthenticateFailureEvent : BankIdEvent
    {
        internal BankIdAspNetAuthenticateFailureEvent(string errorReason)
            : base(BankIdEventTypes.BankIdAspNetAuthenticateErrorEventId, BankIdEventTypes.BankIdAspNetAuthenticateFailureEventName, EventSeverity.Failure)
        {
            ErrorReason = errorReason;
        }

        public string ErrorReason { get; }
    }
}
