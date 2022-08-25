using ActiveLogin.Authentication.BankId.Core.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.Core.SupportedDevice;

namespace ActiveLogin.Authentication.BankId.Core.Events;

/// <summary>
/// Event for when handling the ASP.NET authentication fails.
/// </summary>
public class BankIdAspNetAuthenticateFailureEvent : BankIdEvent
{
    public BankIdAspNetAuthenticateFailureEvent(string errorReason, BankIdSupportedDevice detectedUserDevice)
        : base(BankIdEventTypes.AspNetAuthenticateErrorEventId, BankIdEventTypes.AspNetAuthenticateFailureEventName, BankIdEventSeverity.Failure)
    {
        ErrorReason = errorReason;
        DetectedUserDevice = detectedUserDevice;
    }

    public string ErrorReason { get; }

    public BankIdSupportedDevice DetectedUserDevice { get; }
}
