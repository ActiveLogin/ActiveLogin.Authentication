using ActiveLogin.Authentication.BankId.Core.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.Core.Models;
using ActiveLogin.Authentication.BankId.Core.SupportedDevice;

namespace ActiveLogin.Authentication.BankId.Core.Events;

/// <summary>
/// Event for successfully canceling authentication order.
/// </summary>
public class BankIdCancelSuccessEvent : BankIdEvent
{
    internal BankIdCancelSuccessEvent(string orderRef, BankIdSupportedDevice detectedUserDevice, BankIdFlowOptions idOptions)
        : base(BankIdEventTypes.CancelSuccessId, BankIdEventTypes.CancelSuccessName, BankIdEventSeverity.Success)
    {
        OrderRef = orderRef;
        DetectedUserDevice = detectedUserDevice;
        BankIdOptions = idOptions;
    }

    public string OrderRef { get; }

    public BankIdSupportedDevice DetectedUserDevice { get; }

    public BankIdFlowOptions BankIdOptions { get; }
}
