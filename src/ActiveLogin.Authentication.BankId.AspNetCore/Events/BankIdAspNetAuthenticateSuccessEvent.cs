using ActiveLogin.Identity.Swedish;
using Microsoft.AspNetCore.Authentication;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    /// <summary>
    /// Event for creating an ASP.NET authentication ticket.
    /// </summary>
    public class BankIdAspNetAuthenticateSuccessEvent : BankIdEvent
    {
        internal BankIdAspNetAuthenticateSuccessEvent(AuthenticationTicket authenticationTicket, SwedishPersonalIdentityNumber personalIdentityNumber)
            : base(BankIdEventTypes.BankIdAspNetAuthenticateSuccessEventId, BankIdEventTypes.BankIdAspNetAuthenticateSuccessEventName, EventSeverity.Success)
        {
            AuthenticationTicket = authenticationTicket;
            PersonalIdentityNumber = personalIdentityNumber;
        }

        public AuthenticationTicket AuthenticationTicket { get; }
        public SwedishPersonalIdentityNumber PersonalIdentityNumber { get; }
    }
}
