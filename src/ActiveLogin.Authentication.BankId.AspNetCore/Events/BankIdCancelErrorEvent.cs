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
        internal BankIdCancelErrorEvent(string orderRef, BankIdApiException bankIdApiException)
            : base(BankIdEventTypes.CancelFailureId, BankIdEventTypes.CancelFailureName, BankIdEventSeverity.Error)
        {
            OrderRef = orderRef;
            BankIdApiException = bankIdApiException;
        }

        public string OrderRef { get; }

        public BankIdApiException BankIdApiException { get; }
    }
}
