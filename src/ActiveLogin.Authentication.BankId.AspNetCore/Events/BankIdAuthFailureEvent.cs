using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Identity.Swedish;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    /// <summary>
    /// Event for failed initiation of authentication order. 
    /// </summary>
    public class BankIdAuthFailureEvent : BankIdEvent
    {
        public BankIdAuthFailureEvent(SwedishPersonalIdentityNumber? personalIdentityNumber, BankIdApiException bankIdApiException)
            : base(BankIdEventTypeIds.BankIdAuthHardFailure, BankIdEventTypeNames.BankIdAuthHardFailure, EventSeverity.Error)
        {
            PersonalIdentityNumber = personalIdentityNumber;
            BankIdApiException = bankIdApiException;
        }

        public SwedishPersonalIdentityNumber? PersonalIdentityNumber { get; set; }

        public BankIdApiException BankIdApiException { get; set; }
    }
}

