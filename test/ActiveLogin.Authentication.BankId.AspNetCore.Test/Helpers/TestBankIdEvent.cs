using ActiveLogin.Authentication.BankId.AspNetCore.Events.Infrastructure;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Test.Helpers
{
    public class TestBankIdEvent : BankIdEvent
    {
        internal TestBankIdEvent(int eventTypeId, string eventTypeName, EventSeverity eventSeverity) : base(eventTypeId, eventTypeName, eventSeverity)
        {
        }
    }
}
