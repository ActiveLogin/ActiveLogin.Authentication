using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.Core.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.Core.Models;
using ActiveLogin.Authentication.BankId.Core.SupportedDevice;

namespace ActiveLogin.Authentication.BankId.Core.Events;

/// <summary>
/// Event for collect pending authentication order.
/// </summary>
public class BankIdCollectPendingEvent : BankIdEvent
{
    internal BankIdCollectPendingEvent(string orderRef, CollectHintCode hintCode, BankIdSupportedDevice detectedUserDevice, BankIdLoginOptions idOptions)
        : base(BankIdEventTypes.CollectPendingId, BankIdEventTypes.CollectPendingName, BankIdEventSeverity.Information)
    {
        OrderRef = orderRef;
        HintCode = hintCode;
        DetectedUserDevice = detectedUserDevice;
        BankIdOptions = idOptions;
    }

    public string OrderRef { get; }

    public CollectHintCode HintCode { get; }

    public BankIdSupportedDevice DetectedUserDevice { get; }

    public BankIdLoginOptions BankIdOptions { get; }
}
