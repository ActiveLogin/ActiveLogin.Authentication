using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.Core.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.Core.Models;
using ActiveLogin.Authentication.BankId.Core.SupportedDevice;

namespace ActiveLogin.Authentication.BankId.Core.Events;

/// <summary>
/// Event for canceling authentication order failed.
/// </summary>
public class BankIdCancelErrorEvent : BankIdEvent
{
    internal BankIdCancelErrorEvent(string orderRef, BankIdApiException bankIdApiException, BankIdSupportedDevice detectedUserDevice, BankIdLoginOptions idOptions)
        : base(BankIdEventTypes.CancelFailureId, BankIdEventTypes.CancelFailureName, BankIdEventSeverity.Error)
    {
        OrderRef = orderRef;
        BankIdApiException = bankIdApiException;
        DetectedUserDevice = detectedUserDevice;
        BankIdOptions = idOptions;
    }

    public string OrderRef { get; }

    public BankIdApiException BankIdApiException { get; }

    public BankIdSupportedDevice DetectedUserDevice { get; }

    public BankIdLoginOptions BankIdOptions { get; }
}
