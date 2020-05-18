using ActiveLogin.Authentication.BankId.AspNetCore.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice;
using ActiveLogin.Identity.Swedish;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    /// <summary>
    /// Event for successful initiation of authentication order. 
    /// </summary>
    public class BankIdAuthSuccessEvent : BankIdEvent
    {
        internal BankIdAuthSuccessEvent(SwedishPersonalIdentityNumber? personalIdentityNumber, string orderRef, BankIdSupportedDevice detectedUserDevice)
            : base(BankIdEventTypes.AuthSuccessId, BankIdEventTypes.AuthSuccessName, BankIdEventSeverity.Success)
        {
            PersonalIdentityNumber = personalIdentityNumber;
            OrderRef = orderRef;
            DetectedUserDevice = detectedUserDevice;
        }

        public SwedishPersonalIdentityNumber? PersonalIdentityNumber { get; }

        public string OrderRef { get; }

        public BankIdSupportedDevice DetectedUserDevice { get; }
    }
}

