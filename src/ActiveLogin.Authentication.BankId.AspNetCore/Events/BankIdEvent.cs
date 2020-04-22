namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    public abstract class BankIdEvent
    {
        protected BankIdEvent(int eventTypeId, string eventTypeName, EventSeverity severity)
        {
            EventTypeId = eventTypeId;
            EventTypeName = eventTypeName;
            Severity = severity;
        }

        public int EventTypeId { get; }

        public string EventTypeName { get; }

        public EventSeverity Severity { get; }
    }
}
