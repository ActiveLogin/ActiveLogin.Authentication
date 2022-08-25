using ActiveLogin.Authentication.BankId.Core.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.Core.SupportedDevice;

namespace ActiveLogin.Authentication.BankId.Core.Events;

/// <summary>
/// Event for when handling the sign fails.
/// </summary>
public class BankIdSignFailureEvent : BankIdEvent
{
    public BankIdSignFailureEvent(string errorReason, BankIdSupportedDevice detectedUserDevice)
        : base(BankIdEventTypes.SignErrorEventId, BankIdEventTypes.SignErrorEventName, BankIdEventSeverity.Failure)
    {
        ErrorReason = errorReason;
        DetectedUserDevice = detectedUserDevice;
    }

    public string ErrorReason { get; }

    public BankIdSupportedDevice DetectedUserDevice { get; }
}
