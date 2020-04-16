using ActiveLogin.Authentication.BankId.Api;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    /// <summary>
    /// Event for collect error.
    /// </summary>
    public class BankIdCollectErrorEvent : BankIdEvent
    {
        public BankIdCollectErrorEvent(string orderRef, BankIdApiException bankIdApiException)
            : base(BankIdEventTypeIds.BankIdCollectHardFailure, BankIdEventTypeNames.BankIdCollectHardFailure, EventSeverity.Error)
        {
            OrderRef = orderRef;
            BankIdApiException = bankIdApiException;
        }

        public string OrderRef { get; set; }

        public BankIdApiException BankIdApiException { get; set; }

    }
}

