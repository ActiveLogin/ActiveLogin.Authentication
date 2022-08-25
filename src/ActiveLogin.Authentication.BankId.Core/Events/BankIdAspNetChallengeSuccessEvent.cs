using ActiveLogin.Authentication.BankId.Core.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.Core.Models;
using ActiveLogin.Authentication.BankId.Core.SupportedDevice;

namespace ActiveLogin.Authentication.BankId.Core.Events;

/// <summary>
/// Event for creating an ASP.NET authentication ticket.
/// </summary>
public class BankIdAspNetChallengeSuccessEvent : BankIdEvent
{
    public BankIdAspNetChallengeSuccessEvent(BankIdSupportedDevice detectedUserDevice, BankIdFlowOptions bankIdOptions)
        : base(BankIdEventTypes.AspNetChallengeSuccessEventId, BankIdEventTypes.AspNetChallengeSuccessEventName, BankIdEventSeverity.Success)
    {
        DetectedUserDevice = detectedUserDevice;
        BankIdOptions = bankIdOptions;
    }

    public BankIdSupportedDevice DetectedUserDevice { get; }

    public BankIdFlowOptions BankIdOptions { get; }
}
