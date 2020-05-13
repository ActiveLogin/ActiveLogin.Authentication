using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.AspNetCore.Events.Infrastructure;
using ActiveLogin.Identity.Swedish;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    /// <summary>
    /// Event for failed initiation of authentication order. 
    /// </summary>
    public class BankIdAuthErrorEvent : BankIdEvent
    {
        internal BankIdAuthErrorEvent(SwedishPersonalIdentityNumber? personalIdentityNumber, BankIdApiException bankIdApiException)
            : base(BankIdEventTypes.BankIdAuthErrorEventId, BankIdEventTypes.BankIdAuthErrorEventName, EventSeverity.Error)
        {
            PersonalIdentityNumber = personalIdentityNumber;
            BankIdApiException = bankIdApiException;
        }

        public SwedishPersonalIdentityNumber? PersonalIdentityNumber { get; }

        public BankIdApiException BankIdApiException { get; }
    }
}
