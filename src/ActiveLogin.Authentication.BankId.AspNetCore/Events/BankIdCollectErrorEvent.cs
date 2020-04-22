using ActiveLogin.Authentication.BankId.Api;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    /// <summary>
    /// Event for collect error.
    /// </summary>
    public class BankIdCollectErrorEvent : BankIdEvent
    {
        internal BankIdCollectErrorEvent(string orderRef, BankIdApiException bankIdApiException)
            : base(BankIdEventTypes.BankIdCollectHardFailureId, BankIdEventTypes.BankIdCollectHardFailureName, EventSeverity.Error)
        {
            OrderRef = orderRef;
            BankIdApiException = bankIdApiException;
        }

        public string OrderRef { get; }

        public BankIdApiException BankIdApiException { get; }
    }
}
