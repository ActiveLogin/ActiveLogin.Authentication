using System.Text.Json.Serialization;
using ActiveLogin.Authentication.BankId.AspNetCore.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice;
using ActiveLogin.Identity.Swedish;
using Microsoft.AspNetCore.Authentication;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    /// <summary>
    /// Event for creating an ASP.NET authentication ticket.
    /// </summary>
    public class BankIdAspNetAuthenticateSuccessEvent : BankIdEvent
    {
        internal BankIdAspNetAuthenticateSuccessEvent(AuthenticationTicket authenticationTicket, PersonalIdentityNumber personalIdentityNumber, BankIdSupportedDevice detectedUserDevice)
            : base(BankIdEventTypes.AspNetAuthenticateSuccessEventId, BankIdEventTypes.AspNetAuthenticateSuccessEventName, BankIdEventSeverity.Success)
        {
            AuthenticationTicket = authenticationTicket;
            PersonalIdentityNumber = personalIdentityNumber;
            DetectedUserDevice = detectedUserDevice;
        }

        [JsonIgnore] // ClaimsPrincipal have self circular references
        public AuthenticationTicket AuthenticationTicket { get; }
        public PersonalIdentityNumber PersonalIdentityNumber { get; }

        public BankIdSupportedDevice DetectedUserDevice { get; }
    }
}
