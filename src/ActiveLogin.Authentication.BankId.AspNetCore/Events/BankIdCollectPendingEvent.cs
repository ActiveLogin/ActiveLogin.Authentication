using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    /// <summary>
    /// Event for collect pending authentication order.
    /// </summary>
    public class BankIdCollectPendingEvent : BankIdEvent
    {
        internal BankIdCollectPendingEvent(string orderRef, CollectHintCode hintCode, BankIdSupportedDevice detectedUserDevice)
            : base(BankIdEventTypes.CollectPendingId, BankIdEventTypes.CollectPendingName, BankIdEventSeverity.Information)
        {
            OrderRef = orderRef;
            HintCode = hintCode;
            DetectedUserDevice = detectedUserDevice;
        }

        public string OrderRef { get; }

        public CollectHintCode HintCode { get; }

        public BankIdSupportedDevice DetectedUserDevice { get; }
    }
}
