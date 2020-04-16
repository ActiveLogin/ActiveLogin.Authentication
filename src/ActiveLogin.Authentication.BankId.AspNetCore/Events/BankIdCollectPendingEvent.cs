using ActiveLogin.Authentication.BankId.Api.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    /// <summary>
    /// Event for collect pending authentication order.
    /// </summary>
    public class BankIdCollectPendingEvent : BankIdEvent
    {
        public BankIdCollectPendingEvent(string orderRef, CollectHintCode hintCode)
            : base(BankIdEventTypeIds.BankIdCollectPending, BankIdEventTypeNames.BankIdCollectPending, EventSeverity.Information)
        {
            OrderRef = orderRef;
            HintCode = hintCode;
        }

        public string OrderRef { get; set; }

        public CollectHintCode HintCode { get; set; }
    }
}

