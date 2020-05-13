using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Events.Infrastructure;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    /// <summary>
    /// Event for collect failed authentication order.
    /// </summary>
    public class BankIdCollectFailureEvent : BankIdEvent
    {
        internal BankIdCollectFailureEvent(string orderRef, CollectHintCode hintCode)
            : base(BankIdEventTypes.BankIdCollectFailureId, BankIdEventTypes.BankIdCollectFailureName, EventSeverity.Failure)
        {
            OrderRef = orderRef;
            HintCode = hintCode;
        }

        public string OrderRef { get; }

        public CollectHintCode HintCode { get; }
    }
}
