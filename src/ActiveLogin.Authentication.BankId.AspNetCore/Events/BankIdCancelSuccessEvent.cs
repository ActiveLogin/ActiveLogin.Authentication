namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    /// <summary>
    /// Event for successfully canceling authentication order.
    /// </summary>
    public class BankIdCancelSuccessEvent : BankIdEvent
    {
        public BankIdCancelSuccessEvent(string orderRef)
            : base(BankIdEventTypeIds.BankIdCancelSuccess, BankIdEventTypeNames.BankIdCancelSuccess, EventSeverity.Success)
        {
            OrderRef = orderRef;
        }

        public string OrderRef { get; set; }

    }
}

