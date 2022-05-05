using ActiveLogin.Authentication.BankId.Core.Events.Infrastructure;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Test.Helpers;

public class TestBankIdEvent : BankIdEvent
{
    internal TestBankIdEvent(int eventTypeId, string eventTypeName, BankIdEventSeverity eventSeverity) : base(eventTypeId, eventTypeName, eventSeverity)
    {
    }
}