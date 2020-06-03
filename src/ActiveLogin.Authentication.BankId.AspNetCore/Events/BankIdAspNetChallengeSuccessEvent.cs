using ActiveLogin.Authentication.BankId.AspNetCore.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    /// <summary>
    /// Event for creating an ASP.NET authentication ticket.
    /// </summary>
    public class BankIdAspNetChallengeSuccessEvent : BankIdEvent
    {
        internal BankIdAspNetChallengeSuccessEvent(BankIdSupportedDevice detectedUserDevice, BankIdLoginOptions bankIdOptions)
            : base(BankIdEventTypes.AspNetChallengeSuccessEventId, BankIdEventTypes.AspNetChallengeSuccessEventName, BankIdEventSeverity.Success)
        {
            DetectedUserDevice = detectedUserDevice;
            BankIdOptions = bankIdOptions;
        }

        public BankIdSupportedDevice DetectedUserDevice { get; }

        public BankIdLoginOptions BankIdOptions { get; }
    }
}
