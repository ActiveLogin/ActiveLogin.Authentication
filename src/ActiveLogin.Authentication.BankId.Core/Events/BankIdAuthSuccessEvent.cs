using ActiveLogin.Authentication.BankId.Core.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.Core.Models;
using ActiveLogin.Authentication.BankId.Core.SupportedDevice;
using ActiveLogin.Identity.Swedish;

namespace ActiveLogin.Authentication.BankId.Core.Events;

/// <summary>
/// Event for successful initiation of authentication order. 
/// </summary>
public class BankIdAuthSuccessEvent : BankIdEvent
{
    internal BankIdAuthSuccessEvent(PersonalIdentityNumber? personalIdentityNumber, string orderRef, BankIdSupportedDevice detectedUserDevice, BankIdFlowOptions idOptions)
        : base(BankIdEventTypes.AuthSuccessId, BankIdEventTypes.AuthSuccessName, BankIdEventSeverity.Success)
    {
        PersonalIdentityNumber = personalIdentityNumber;
        OrderRef = orderRef;
        DetectedUserDevice = detectedUserDevice;
        BankIdOptions = idOptions;
    }

    public PersonalIdentityNumber? PersonalIdentityNumber { get; }

    public string OrderRef { get; }

    public BankIdSupportedDevice DetectedUserDevice { get; }

    public BankIdFlowOptions BankIdOptions { get; }
}
