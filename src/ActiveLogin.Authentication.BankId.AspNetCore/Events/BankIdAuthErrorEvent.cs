using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.AspNetCore.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice;
using ActiveLogin.Identity.Swedish;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    /// <summary>
    /// Event for failed initiation of authentication order. 
    /// </summary>
    public class BankIdAuthErrorEvent : BankIdEvent
    {
        internal BankIdAuthErrorEvent(PersonalIdentityNumber? personalIdentityNumber, BankIdApiException bankIdApiException, BankIdSupportedDevice detectedUserDevice, BankIdLoginOptions idOptions)
            : base(BankIdEventTypes.AuthErrorEventId, BankIdEventTypes.AuthErrorEventName, BankIdEventSeverity.Error)
        {
            PersonalIdentityNumber = personalIdentityNumber;
            BankIdApiException = bankIdApiException;
            DetectedUserDevice = detectedUserDevice;
            BankIdOptions = idOptions;
        }

        public PersonalIdentityNumber? PersonalIdentityNumber { get; }

        public BankIdApiException BankIdApiException { get; }

        public BankIdSupportedDevice DetectedUserDevice { get; }

        public BankIdLoginOptions BankIdOptions { get; }
    }
}
