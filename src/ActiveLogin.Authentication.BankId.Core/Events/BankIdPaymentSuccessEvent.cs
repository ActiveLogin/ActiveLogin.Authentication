using ActiveLogin.Authentication.BankId.Core.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.Core.SupportedDevice;
using ActiveLogin.Identity.Swedish;

namespace ActiveLogin.Authentication.BankId.Core.Events;

/// <summary>
/// Event for successful initiation of payment order.
/// </summary>
public class BankIdPaymentSuccessEvent : BankIdEvent
{
    public BankIdPaymentSuccessEvent(PersonalIdentityNumber personalIdentityNumber, BankIdSupportedDevice detectedUserDevice)
        : base(BankIdEventTypes.PaymentSuccessId, BankIdEventTypes.PaymentSuccessName, BankIdEventSeverity.Success)
    {
        PersonalIdentityNumber = personalIdentityNumber;
        DetectedUserDevice = detectedUserDevice;
    }
        
    public PersonalIdentityNumber PersonalIdentityNumber { get; }

    public BankIdSupportedDevice DetectedUserDevice { get; }
}
