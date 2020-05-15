using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.AspNetCore.Events.Infrastructure;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    /// <summary>
    /// Event for collect error.
    /// </summary>
    public class BankIdCollectErrorEvent : BankIdEvent
    {
        internal BankIdCollectErrorEvent(string orderRef, BankIdApiException bankIdApiException)
            : base(BankIdEventTypes.CollectErrorId, BankIdEventTypes.CollectErrorName, BankIdEventSeverity.Error)
        {
            OrderRef = orderRef;
            BankIdApiException = bankIdApiException;
        }

        public string OrderRef { get; }

        public BankIdApiException BankIdApiException { get; }
    }
}
