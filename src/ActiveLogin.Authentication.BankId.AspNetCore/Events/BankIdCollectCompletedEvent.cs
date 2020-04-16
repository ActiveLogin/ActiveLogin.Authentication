using ActiveLogin.Authentication.BankId.Api.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    /// <summary>
    /// Event for collect complete authentication order.
    /// </summary>
    public class BankIdCollectCompletedEvent : BankIdEvent
    {
        public BankIdCollectCompletedEvent(string orderRef, CompletionData completionData)
            : base(BankIdEventTypeIds.BankIdCollectCompleted, BankIdEventTypeNames.BankIdCollectCompleted, EventSeverity.Information)
        {
            OrderRef = orderRef;
            CompletionData = completionData;
        }

        public string OrderRef { get; set; }

        public CompletionData CompletionData { get; set; }

    }
}

