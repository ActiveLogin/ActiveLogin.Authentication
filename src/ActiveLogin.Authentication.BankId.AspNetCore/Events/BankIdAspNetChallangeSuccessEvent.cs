using ActiveLogin.Authentication.BankId.AspNetCore.Models;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    /// <summary>
    /// Event for creating an ASP.NET authentication ticket.
    /// </summary>
    public class BankIdAspNetChallangeSuccessEvent : BankIdEvent
    {
        internal BankIdAspNetChallangeSuccessEvent(BankIdLoginOptions bankIdOptions)
            : base(BankIdEventTypes.BankIdAspNetChallengeSuccessEventId, BankIdEventTypes.BankIdAspNetChallengeSuccessEventName, EventSeverity.Success)
        {
            BankIdOptions = bankIdOptions;
        }

        public BankIdLoginOptions BankIdOptions { get; }
    }
}
