using ActiveLogin.Authentication.BankId.AspNetCore.Events;
using ActiveLogin.Authentication.BankId.AspNetCore.Events.Infrastructure;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Test.Events
{
    public class TestBankIdEvent : BankIdEvent
    {
        internal TestBankIdEvent() : base(9_9_9, "TestBankIdEvent", EventSeverity.Information)
        {
        }
    }
}
