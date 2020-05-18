using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.AspNetCore.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice;
using ActiveLogin.Identity.Swedish;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    /// <summary>
    /// Event for failed initiation of authentication order. 
    /// </summary>
    public class BankIdAuthErrorEvent : BankIdEvent
    {
        internal BankIdAuthErrorEvent(SwedishPersonalIdentityNumber? personalIdentityNumber, BankIdApiException bankIdApiException, BankIdSupportedDevice detectedUserDevice)
            : base(BankIdEventTypes.AuthErrorEventId, BankIdEventTypes.AuthErrorEventName, BankIdEventSeverity.Error)
        {
            PersonalIdentityNumber = personalIdentityNumber;
            BankIdApiException = bankIdApiException;
            DetectedUserDevice = detectedUserDevice;
        }

        public SwedishPersonalIdentityNumber? PersonalIdentityNumber { get; }

        public BankIdApiException BankIdApiException { get; }

        public BankIdSupportedDevice DetectedUserDevice { get; }
    }
}
