using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.Core.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.Core.Models;
using ActiveLogin.Authentication.BankId.Core.SupportedDevice;

namespace ActiveLogin.Authentication.BankId.Core.Events;

/// <summary>
/// Event for collect failed authentication order.
/// </summary>
public class BankIdCollectFailureEvent : BankIdEvent
{
    internal BankIdCollectFailureEvent(string orderRef, CollectHintCode hintCode, BankIdSupportedDevice detectedUserDevice, BankIdFlowOptions idOptions)
        : base(BankIdEventTypes.CollectFailureId, BankIdEventTypes.CollectFailureName, BankIdEventSeverity.Failure)
    {
        OrderRef = orderRef;
        HintCode = hintCode;
        DetectedUserDevice = detectedUserDevice;
        BankIdOptions = idOptions;
    }

    public string OrderRef { get; }

    public CollectHintCode HintCode { get; }

    public BankIdSupportedDevice DetectedUserDevice { get; }

    public BankIdFlowOptions BankIdOptions { get; }
}
