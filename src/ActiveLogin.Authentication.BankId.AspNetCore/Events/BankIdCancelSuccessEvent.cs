using ActiveLogin.Authentication.BankId.AspNetCore.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    /// <summary>
    /// Event for successfully canceling authentication order.
    /// </summary>
    public class BankIdCancelSuccessEvent : BankIdEvent
    {
        internal BankIdCancelSuccessEvent(string orderRef, BankIdSupportedDevice detectedUserDevice, BankIdLoginOptions idOptions)
            : base(BankIdEventTypes.CancelSuccessId, BankIdEventTypes.CancelSuccessName, BankIdEventSeverity.Success)
        {
            OrderRef = orderRef;
            DetectedUserDevice = detectedUserDevice;
            BankIdOptions = idOptions;
        }

        public string OrderRef { get; }

        public BankIdSupportedDevice DetectedUserDevice { get; }

        public BankIdLoginOptions BankIdOptions { get; }
    }
}
