using System.Text.Json.Serialization;
using ActiveLogin.Authentication.BankId.AspNetCore.Events.Infrastructure;
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
            : base(BankIdEventTypes.AspNetAuthenticateSuccessEventId, BankIdEventTypes.AspNetAuthenticateSuccessEventName, BankIdEventSeverity.Success)
        {
            AuthenticationTicket = authenticationTicket;
            PersonalIdentityNumber = personalIdentityNumber;
        }

        [JsonIgnore] // ClaimsPrincipal have self circular references
        public AuthenticationTicket AuthenticationTicket { get; }
        public SwedishPersonalIdentityNumber PersonalIdentityNumber { get; }
    }
}
