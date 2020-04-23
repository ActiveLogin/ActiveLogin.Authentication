namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    public abstract class BankIdEvent
    {
        protected BankIdEvent(int eventTypeId, string eventTypeName, EventSeverity eventSeverity)
        {
            EventTypeId = eventTypeId;
            EventTypeName = eventTypeName;
            EventSeverity = eventSeverity;
        }

        public int EventTypeId { get; }

        public string EventTypeName { get; }

        public EventSeverity EventSeverity { get; }
    }
}
