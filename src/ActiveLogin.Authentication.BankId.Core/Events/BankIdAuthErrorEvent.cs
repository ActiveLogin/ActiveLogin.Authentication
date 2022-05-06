using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.Core.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.Core.Models;
using ActiveLogin.Authentication.BankId.Core.SupportedDevice;
using ActiveLogin.Identity.Swedish;

namespace ActiveLogin.Authentication.BankId.Core.Events;

/// <summary>
/// Event for failed initiation of authentication order. 
/// </summary>
public class BankIdAuthErrorEvent : BankIdEvent
{
    internal BankIdAuthErrorEvent(PersonalIdentityNumber? personalIdentityNumber, BankIdApiException bankIdApiException, BankIdSupportedDevice detectedUserDevice, BankIdFlowOptions idOptions)
        : base(BankIdEventTypes.AuthErrorEventId, BankIdEventTypes.AuthErrorEventName, BankIdEventSeverity.Error)
    {
        PersonalIdentityNumber = personalIdentityNumber;
        BankIdApiException = bankIdApiException;
        DetectedUserDevice = detectedUserDevice;
        BankIdOptions = idOptions;
    }

    public PersonalIdentityNumber? PersonalIdentityNumber { get; }

    public BankIdApiException BankIdApiException { get; }

    public BankIdSupportedDevice DetectedUserDevice { get; }

    public BankIdFlowOptions BankIdOptions { get; }
}
