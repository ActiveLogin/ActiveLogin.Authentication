using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Events.Infrastructure;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    /// <summary>
    /// Event for collect complete authentication order.
    /// </summary>
    public class BankIdCollectCompletedEvent : BankIdEvent
    {
        internal BankIdCollectCompletedEvent(string orderRef, CompletionData completionData)
            : base(BankIdEventTypes.BankIdCollectCompletedId, BankIdEventTypes.BankIdCollectCompletedName, BankIdEventSeverity.Success)
        {
            OrderRef = orderRef;
            CompletionData = completionData;
        }

        public string OrderRef { get; }

        public CompletionData CompletionData { get; }
    }
}
