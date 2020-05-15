using ActiveLogin.Authentication.BankId.AspNetCore.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    /// <summary>
    /// Event for creating an ASP.NET authentication ticket.
    /// </summary>
    public class BankIdAspNetChallengeSuccessEvent : BankIdEvent
    {
        internal BankIdAspNetChallengeSuccessEvent(BankIdLoginOptions bankIdOptions)
            : base(BankIdEventTypes.AspNetChallengeSuccessEventId, BankIdEventTypes.AspNetChallengeSuccessEventName, BankIdEventSeverity.Success)
        {
            BankIdOptions = bankIdOptions;
        }

        public BankIdLoginOptions BankIdOptions { get; }
    }
}
