using ActiveLogin.Authentication.BankId.Api.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    /// <summary>
    /// Event for collect failed authentication order.
    /// </summary>
    public class BankIdCollectFailureEvent : BankIdEvent
    {
        public BankIdCollectFailureEvent(string orderRef, CollectHintCode hintCode)
            : base(BankIdEventTypeIds.BankIdCollectSoftFailure, BankIdEventTypeNames.BankIdCollectSoftFailure, EventSeverity.Failure)
        {
            OrderRef = orderRef;
            HintCode = hintCode;
        }

        public string OrderRef { get; set; }

        public CollectHintCode HintCode { get; set; }

    }
}

