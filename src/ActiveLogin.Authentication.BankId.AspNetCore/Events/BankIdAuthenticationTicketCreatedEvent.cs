using ActiveLogin.Identity.Swedish;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    /// <summary>
    /// Event for creating an authentication ticket.
    /// </summary>
    public class BankIdAuthenticationTicketCreatedEvent : BankIdEvent
    {
        internal BankIdAuthenticationTicketCreatedEvent(SwedishPersonalIdentityNumber personalIdentityNumber)
            : base(BankIdEventTypes.BankIdAuthenticationTicketCreatedId, BankIdEventTypes.BankIdAuthenticationTicketCreatedName, EventSeverity.Information)
        {
            PersonalIdentityNumber = personalIdentityNumber;
        }

        public SwedishPersonalIdentityNumber PersonalIdentityNumber { get; }
    }
}
