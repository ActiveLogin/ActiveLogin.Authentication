using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    /// <summary>
    /// Event for collect failed authentication order.
    /// </summary>
    public class BankIdCollectFailureEvent : BankIdEvent
    {
        internal BankIdCollectFailureEvent(string orderRef, CollectHintCode hintCode, BankIdSupportedDevice detectedUserDevice, BankIdLoginOptions idOptions)
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

        public BankIdLoginOptions BankIdOptions { get; }
    }
}
