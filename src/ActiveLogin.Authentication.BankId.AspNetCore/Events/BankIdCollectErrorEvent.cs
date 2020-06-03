using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.AspNetCore.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    /// <summary>
    /// Event for collect error.
    /// </summary>
    public class BankIdCollectErrorEvent : BankIdEvent
    {
        internal BankIdCollectErrorEvent(string orderRef, BankIdApiException bankIdApiException, BankIdSupportedDevice detectedUserDevice, BankIdLoginOptions idOptions)
            : base(BankIdEventTypes.CollectErrorId, BankIdEventTypes.CollectErrorName, BankIdEventSeverity.Error)
        {
            OrderRef = orderRef;
            BankIdApiException = bankIdApiException;
            DetectedUserDevice = detectedUserDevice;
            BankIdOptions = idOptions;
        }

        public string OrderRef { get; }

        public BankIdApiException BankIdApiException { get; }

        public BankIdSupportedDevice DetectedUserDevice { get; }

        public BankIdLoginOptions BankIdOptions { get; }
    }
}
