namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    /// <summary>
    /// Event for successfully canceling authentication order.
    /// </summary>
    public class BankIdCancelSuccessEvent : BankIdEvent
    {
        internal BankIdCancelSuccessEvent(string orderRef)
            : base(BankIdEventTypes.BankIdCancelSuccessId, BankIdEventTypes.BankIdCancelSuccessName, EventSeverity.Success)
        {
            OrderRef = orderRef;
        }

        public string OrderRef { get; }
    }
}
