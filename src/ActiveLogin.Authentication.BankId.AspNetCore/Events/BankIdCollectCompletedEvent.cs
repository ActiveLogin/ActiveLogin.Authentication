using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.Events.Infrastructure;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.BankId.AspNetCore.SupportedDevice;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    /// <summary>
    /// Event for collect complete authentication order.
    /// </summary>
    public class BankIdCollectCompletedEvent : BankIdEvent
    {
        internal BankIdCollectCompletedEvent(string orderRef, CompletionData completionData, BankIdSupportedDevice detectedUserDevice, BankIdLoginOptions idOptions)
            : base(BankIdEventTypes.CollectCompletedId, BankIdEventTypes.CollectCompletedName, BankIdEventSeverity.Success)
        {
            OrderRef = orderRef;
            CompletionData = completionData;
            DetectedUserDevice = detectedUserDevice;
            BankIdOptions = idOptions;
        }

        public string OrderRef { get; }

        public CompletionData CompletionData { get; }

        public BankIdSupportedDevice DetectedUserDevice { get; }

        public BankIdLoginOptions BankIdOptions { get; }
    }
}
