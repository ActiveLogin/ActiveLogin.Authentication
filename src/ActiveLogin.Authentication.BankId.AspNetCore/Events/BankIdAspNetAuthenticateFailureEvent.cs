using ActiveLogin.Authentication.BankId.AspNetCore.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    /// <summary>
    /// Event for when handling the ASP.NET authentication fails.
    /// </summary>
    public class BankIdAspNetAuthenticateFailureEvent : BankIdEvent
    {
        internal BankIdAspNetAuthenticateFailureEvent(string errorReason, BankIdSupportedDevice detectedUserDevice)
            : base(BankIdEventTypes.AspNetAuthenticateErrorEventId, BankIdEventTypes.AspNetAuthenticateFailureEventName, BankIdEventSeverity.Failure)
        {
            ErrorReason = errorReason;
            DetectedUserDevice = detectedUserDevice;
        }

        public string ErrorReason { get; }

        public BankIdSupportedDevice DetectedUserDevice { get; }
    }
}
