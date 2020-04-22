using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    public class LoggerBankIdEventListner : TypedBankIdEventListner
    {
        private const string MissingPersonalIdentityNumber = "-";
        private readonly ILogger<LoggerBankIdEventListner> _logger;

        public LoggerBankIdEventListner(ILogger<LoggerBankIdEventListner> logger)
        {
            _logger = logger;
        }

        // BankID API - Auth

        public override Task HandleBankIdAuthenticationTicketCreatedEvent(BankIdAuthenticationTicketCreatedEvent e)
        {
            _logger.LogInformation(new EventId(e.EventTypeId, e.EventTypeName), "BankID authentication ticket created");
            _logger.LogTrace(new EventId(e.EventTypeId, e.EventTypeName), "BankID authentication ticket created for PersonalIdentityNumber '{PersonalIdentityNumber}'", e.PersonalIdentityNumber.To12DigitString() ?? MissingPersonalIdentityNumber);
            return Task.CompletedTask;
        }

        public override Task HandleBankIdAuthFailureEvent(BankIdAuthFailureEvent e)
        {
            _logger.LogError(e.BankIdApiException, "BankID auth failed with the error '{ErrorCode}' and the details '{ErrorDetails}'", e.BankIdApiException.ErrorCode, e.BankIdApiException.ErrorDetails);
            _logger.LogTrace(new EventId(e.EventTypeId, e.EventTypeName), "BankID auth failed for PersonalIdentityNumber '{PersonalIdentityNumber}' with the error '{ErrorCode}' and the details '{ErrorDetails}'", e.PersonalIdentityNumber?.To12DigitString() ?? MissingPersonalIdentityNumber, e.BankIdApiException.ErrorCode, e.BankIdApiException.ErrorDetails);
            return Task.CompletedTask;
        }

        public override Task HandleBankIdAuthSuccessEvent(BankIdAuthSuccessEvent e)
        {
            _logger.LogInformation(new EventId(e.EventTypeId, e.EventTypeName), "BankID auth succeeded with the OrderRef '{OrderRef}'", e.OrderRef);
            _logger.LogTrace(new EventId(e.EventTypeId, e.EventTypeName), "BankID auth succeeded for PersonalIdentityNumber '{PersonalIdentityNumber}' with the OrderRef '{OrderRef}'", e.PersonalIdentityNumber?.To12DigitString() ?? MissingPersonalIdentityNumber, e.OrderRef);
            return Task.CompletedTask;
        }

        // BankID API - Collect

        public override Task HandleBankIdCollectHardFailureEvent(BankIdCollectErrorEvent e)
        {
            _logger.LogError(e.BankIdApiException, "BankID collect failed for OrderRef '{OrderRef}' with the error '{ErrorCode}' and the details '{ErrorDetails}'", e.OrderRef, e.BankIdApiException.ErrorCode, e.BankIdApiException.ErrorDetails);
            return Task.CompletedTask;
        }

        public override Task HandleBankIdCollectSoftFailureEvent(BankIdCollectFailureEvent e)
        {
            _logger.LogInformation(new EventId(e.EventTypeId, e.EventTypeName), "BankID collect failed for OrderRef '{OrderRef}' with the HintCode '{CollectHintCode}'", e.OrderRef, e.HintCode);
            return Task.CompletedTask;
        }

        public override Task HandleBankIdCollectPendingEvent(BankIdCollectPendingEvent e)
        {
            _logger.LogInformation(new EventId(e.EventTypeId, e.EventTypeName), "BankID collect is pending for OrderRef '{OrderRef}' with HintCode '{CollectHintCode}'", e.OrderRef, e.HintCode);
            return Task.CompletedTask;
        }

        public override Task HandleBankIdCollectCompletedEvent(BankIdCollectCompletedEvent e)
        {
            _logger.LogInformation(new EventId(e.EventTypeId, e.EventTypeName), "BankID collect is completed for OrderRef '{OrderRef}'", e.OrderRef);
            _logger.LogTrace(new EventId(e.EventTypeId, e.EventTypeName), "BankID collect is completed for OrderRef '{OrderRef}' with User (PersonalIdentityNumber: '{UserPersonalIdentityNumber}'; GivenName: '{UserGivenName}'; Surname: '{UserSurname}'; Name: '{UserName}'), Signature '{Signature}' and OcspResponse '{OcspResponse}'", e.OrderRef, e.CompletionData.User.PersonalIdentityNumber, e.CompletionData.User.GivenName, e.CompletionData.User.Surname, e.CompletionData.User.Name, e.CompletionData.Signature, e.CompletionData.OcspResponse);
            return Task.CompletedTask;
        }

        // BankID - Cancel

        public override Task HandleBankIdCancelSuccessEvent(BankIdCancelSuccessEvent e)
        {
            _logger.LogInformation(new EventId(e.EventTypeId, e.EventTypeName), "BankID auth was cancelled with the OrderRef '{OrderRef}'", e.OrderRef);
            return Task.CompletedTask;
        }

        public override Task HandleBankIdCancelFailedEvent(BankIdCancelFailedEvent e)
        {
            _logger.LogInformation(new EventId(e.EventTypeId, e.EventTypeName), "BankID auth cancellation for '{OrderRef}' failed with the message '{Message}'", e.OrderRef, e.Exception.Message);
            return Task.CompletedTask;
        }
    }
}
