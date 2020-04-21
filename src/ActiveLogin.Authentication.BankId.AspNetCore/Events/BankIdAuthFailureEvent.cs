using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Identity.Swedish;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    /// <summary>
    /// Event for failed initiation of authentication order. 
    /// </summary>
    public class BankIdAuthFailureEvent : BankIdEvent
    {
        internal BankIdAuthFailureEvent(SwedishPersonalIdentityNumber? personalIdentityNumber, BankIdApiException bankIdApiException)
            : base(BankIdEventTypes.BankIdAuthHardFailureId, BankIdEventTypes.BankIdAuthHardFailureName, EventSeverity.Error)
        {
            PersonalIdentityNumber = personalIdentityNumber;
            BankIdApiException = bankIdApiException;
        }

        public SwedishPersonalIdentityNumber? PersonalIdentityNumber { get; }

        public BankIdApiException BankIdApiException { get; }
    }
}
