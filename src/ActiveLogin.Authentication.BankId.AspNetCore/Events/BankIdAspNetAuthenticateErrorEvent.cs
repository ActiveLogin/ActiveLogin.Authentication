namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    /// <summary>
    /// Event for when handling the ASP.NET authentication fails.
    /// </summary>
    public class BankIdAspNetAuthenticateErrorEvent : BankIdEvent
    {
        internal BankIdAspNetAuthenticateErrorEvent(string errorReason)
            : base(BankIdEventTypes.BankIdAspNetAuthenticateErrorEventId, BankIdEventTypes.BankIdAspNetAuthenticateErrorEventName, EventSeverity.Error)
        {
            ErrorReason = errorReason;
        }

        public string ErrorReason { get; }
    }
}