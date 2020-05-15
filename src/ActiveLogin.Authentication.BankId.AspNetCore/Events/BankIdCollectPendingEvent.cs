using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Events.Infrastructure;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    /// <summary>
    /// Event for collect pending authentication order.
    /// </summary>
    public class BankIdCollectPendingEvent : BankIdEvent
    {
        internal BankIdCollectPendingEvent(string orderRef, CollectHintCode hintCode)
            : base(BankIdEventTypes.CollectPendingId, BankIdEventTypes.CollectPendingName, BankIdEventSeverity.Information)
        {
            OrderRef = orderRef;
            HintCode = hintCode;
        }

        public string OrderRef { get; }

        public CollectHintCode HintCode { get; }
    }
}
