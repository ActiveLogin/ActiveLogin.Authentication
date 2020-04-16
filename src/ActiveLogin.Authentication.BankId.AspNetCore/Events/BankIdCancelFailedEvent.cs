using System;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    /// <summary>
    /// Event for canceling authentication order failed.
    /// </summary>
    public class BankIdCancelFailedEvent : BankIdEvent
    {
        public BankIdCancelFailedEvent(string orderRef, Exception exception)
            : base(BankIdEventTypeIds.BankIdCancelFailure, BankIdEventTypeNames.BankIdCancelFailure, EventSeverity.Failure)
        {
            OrderRef = orderRef;
            Exception = exception;
        }

        public string OrderRef { get; set; }

        public Exception Exception { get; set; }
    }
}

