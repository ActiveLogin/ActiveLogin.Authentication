using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.Core.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.Core.Models;
using ActiveLogin.Authentication.BankId.Core.SupportedDevice;

namespace ActiveLogin.Authentication.BankId.Core.Events;

/// <summary>
/// Event for collect error.
/// </summary>
public class BankIdCollectErrorEvent : BankIdEvent
{
    internal BankIdCollectErrorEvent(string orderRef, BankIdApiException bankIdApiException, BankIdSupportedDevice detectedUserDevice, BankIdLoginOptions idOptions)
        : base(BankIdEventTypes.CollectErrorId, BankIdEventTypes.CollectErrorName, BankIdEventSeverity.Error)
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
