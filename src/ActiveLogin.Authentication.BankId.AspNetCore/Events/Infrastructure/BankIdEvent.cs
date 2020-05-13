using Microsoft.Extensions.DependencyInjection;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events.Infrastructure
{
    public abstract class BankIdEvent
    {
        protected BankIdEvent(int eventTypeId, string eventTypeName, EventSeverity eventSeverity)
        {
            EventTypeId = eventTypeId;
            EventTypeName = eventTypeName;
            EventSeverity = eventSeverity;
        }

        internal void SetContext(BankIdActiveLoginContext context)
        {
            ActiveLoginProductName = context.ActiveLoginProductName;
            ActiveLoginProductVersion = context.ActiveLoginProductVersion;

            BankIdApiEnvironment = context.BankIdApiEnvironment;
            BankIdApiVersion = context.BankIdApiVersion;
        }

        public int EventTypeId { get; }

        public string EventTypeName { get; }

        public EventSeverity EventSeverity { get; }

        public string ActiveLoginProductName { get; private set; } = string.Empty;
        public string ActiveLoginProductVersion { get; private set; } = string.Empty;

        public string BankIdApiEnvironment { get; private set; } = string.Empty;
        public string BankIdApiVersion { get; private set; } = string.Empty;
    }
}
