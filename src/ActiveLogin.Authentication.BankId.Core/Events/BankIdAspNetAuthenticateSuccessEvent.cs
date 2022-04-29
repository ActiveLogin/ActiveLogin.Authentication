using ActiveLogin.Authentication.BankId.Core.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.Core.SupportedDevice;
using ActiveLogin.Identity.Swedish;

namespace ActiveLogin.Authentication.BankId.Core.Events;

/// <summary>
/// Event for creating an ASP.NET authentication ticket.
/// </summary>
public class BankIdAspNetAuthenticateSuccessEvent : BankIdEvent
{
    internal BankIdAspNetAuthenticateSuccessEvent(PersonalIdentityNumber personalIdentityNumber, BankIdSupportedDevice detectedUserDevice)
        : base(BankIdEventTypes.AspNetAuthenticateSuccessEventId, BankIdEventTypes.AspNetAuthenticateSuccessEventName, BankIdEventSeverity.Success)
    {
        PersonalIdentityNumber = personalIdentityNumber;
        DetectedUserDevice = detectedUserDevice;
    }
        
    public PersonalIdentityNumber PersonalIdentityNumber { get; }

    public BankIdSupportedDevice DetectedUserDevice { get; }
}
