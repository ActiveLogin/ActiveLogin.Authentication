using ActiveLogin.Authentication.BankId.Core.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.Core.SupportedDevice;

namespace ActiveLogin.Authentication.BankId.Core.Events;

/// <summary>
/// Event for when handling when the payment fails.
/// </summary>
public class BankIdPaymentFailureEvent : BankIdEvent
{
    public BankIdPaymentFailureEvent(string errorReason, BankIdSupportedDevice detectedUserDevice)
        : base(BankIdEventTypes.PaymentErrorEventId, BankIdEventTypes.PaymentErrorEventName, BankIdEventSeverity.Failure)
    {
        ErrorReason = errorReason;
        DetectedUserDevice = detectedUserDevice;
    }

    public string ErrorReason { get; }

    public BankIdSupportedDevice DetectedUserDevice { get; }
}
