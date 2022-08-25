using ActiveLogin.Authentication.BankId.Core.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.Core.SupportedDevice;
using ActiveLogin.Identity.Swedish;

namespace ActiveLogin.Authentication.BankId.Core.Events;

/// <summary>
/// Event for creating an ASP.NET authentication ticket.
/// </summary>
public class BankIdSignSuccessEvent : BankIdEvent
{
    public BankIdSignSuccessEvent(PersonalIdentityNumber personalIdentityNumber, BankIdSupportedDevice detectedUserDevice)
        : base(BankIdEventTypes.SignSuccessId, BankIdEventTypes.SignSuccessName, BankIdEventSeverity.Success)
    {
        PersonalIdentityNumber = personalIdentityNumber;
        DetectedUserDevice = detectedUserDevice;
    }
        
    public PersonalIdentityNumber PersonalIdentityNumber { get; }

    public BankIdSupportedDevice DetectedUserDevice { get; }
}
