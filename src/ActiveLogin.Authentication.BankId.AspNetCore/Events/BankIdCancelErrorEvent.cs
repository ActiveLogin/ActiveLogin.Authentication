using System;
using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.AspNetCore.Events.Infrastructure;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    /// <summary>
    /// Event for canceling authentication order failed.
    /// </summary>
    public class BankIdCancelErrorEvent : BankIdEvent
    {
        internal BankIdCancelErrorEvent(string orderRef, BankIdApiException exception)
            : base(BankIdEventTypes.BankIdCancelFailureId, BankIdEventTypes.BankIdCancelFailureName, EventSeverity.Error)
        {
            OrderRef = orderRef;
            Exception = exception;
        }

        public string OrderRef { get; }

        public BankIdApiException Exception { get; }
    }
}
