using ActiveLogin.Authentication.BankId.AspNetCore.Events.Infrastructure;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    /// <summary>
    /// Event for successfully canceling authentication order.
    /// </summary>
    public class BankIdCancelSuccessEvent : BankIdEvent
    {
        internal BankIdCancelSuccessEvent(string orderRef)
            : base(BankIdEventTypes.CancelSuccessId, BankIdEventTypes.CancelSuccessName, BankIdEventSeverity.Success)
        {
            OrderRef = orderRef;
        }

        public string OrderRef { get; }
    }
}
