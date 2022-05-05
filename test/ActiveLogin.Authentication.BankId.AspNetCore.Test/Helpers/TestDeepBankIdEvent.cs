using ActiveLogin.Authentication.BankId.Core.Events.Infrastructure;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Test.Helpers;

public class TestDeepBankIdEvent : BankIdEvent
{
    internal TestDeepBankIdEvent(int eventTypeId, string eventTypeName, BankIdEventSeverity eventSeverity, TestBankIdEvent testEvent) : base(eventTypeId, eventTypeName, eventSeverity)
    {
        TestEvent = testEvent;
    }

    public TestBankIdEvent TestEvent { get; set; }
}