using System;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    /// <summary>
    /// Event for canceling authentication order failed.
    /// </summary>
    public class BankIdCancelFailureEvent : BankIdEvent
    {
        internal BankIdCancelFailureEvent(string orderRef, Exception exception)
            : base(BankIdEventTypes.BankIdCancelFailureId, BankIdEventTypes.BankIdCancelFailureName, EventSeverity.Failure)
        {
            OrderRef = orderRef;
            Exception = exception;
        }

        public string OrderRef { get; }

        public Exception Exception { get; }
    }
}
