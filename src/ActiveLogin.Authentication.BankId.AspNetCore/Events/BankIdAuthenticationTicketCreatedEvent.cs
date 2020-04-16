namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    /// <summary>
    /// Event for creating an authentication ticket.
    /// </summary>
    public class BankIdAuthenticationTicketCreatedEvent : BankIdEvent
    {

        public BankIdAuthenticationTicketCreatedEvent(string personalIdentityNumber)
            : base(BankIdEventTypeIds.BankIdAuthenticationTicketCreated, BankIdEventTypeNames.BankIdAuthenticationTicketCreated, EventSeverity.Information)
        {
            PersonalIdentityNumber = personalIdentityNumber;
        }

        public string PersonalIdentityNumber { get; set; }
    }
}

