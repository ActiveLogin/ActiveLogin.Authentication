using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Identity.Swedish;
using Microsoft.Extensions.Logging;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    internal static class LoggerExtensions
    {
        private const string MissingPersonalIdentityNumber = "-";

        // BankId Authentication Handler

        public static void BankIdAuthenticationTicketCreated(this ILogger logger, string personalIdentityNumber)
        {
            logger.LogInformation(BankIdLoggingEvents.BankIdAuthenticationTicketCreated, "BankID authentication ticket created");
            logger.LogTrace(BankIdLoggingEvents.BankIdAuthenticationTicketCreated, "BankID authentication ticket created for PersonalIdentityNumber '{PersonalIdentityNumber}'", personalIdentityNumber ?? MissingPersonalIdentityNumber);
        }

        // BankID API - Auth

        public static void BankIdAuthFailure(this ILogger logger, SwedishPersonalIdentityNumber personalIdentityNumber, BankIdApiException bankIdApiException)
        {
            logger.LogError(bankIdApiException, "BankID auth failed with the error '{ErrorCode}' and the details '{ErrorDetails}'", bankIdApiException.ErrorCode, bankIdApiException.Details);
            logger.LogTrace(BankIdLoggingEvents.BankIdAuthHardFailure, "BankID auth failed for PersonalIdentityNumber '{PersonalIdentityNumber}' with the error '{ErrorCode}' and the details '{ErrorDetails}'", personalIdentityNumber?.To12DigitString() ?? MissingPersonalIdentityNumber, bankIdApiException.ErrorCode, bankIdApiException.Details);
        }

        public static void BankIdAuthSuccess(this ILogger logger, SwedishPersonalIdentityNumber personalIdentityNumber, string orderRef)
        {
            logger.LogInformation(BankIdLoggingEvents.BankIdAuthSuccess, "BankID auth succedded with the OrderRef '{OrderRef}'", orderRef);
            logger.LogTrace(BankIdLoggingEvents.BankIdAuthSuccess, "BankID auth succedded for PersonalIdentityNumber '{PersonalIdentityNumber}' with the OrderRef '{OrderRef}'", personalIdentityNumber?.To12DigitString() ?? MissingPersonalIdentityNumber, orderRef);
        }

        // BankID API - Collect

        public static void BankIdCollectFailure(this ILogger logger, string orderRef, BankIdApiException bankIdApiException)
        {
            logger.LogError(bankIdApiException, "BankID collect failed for OrderRef '{OrderRef}' with the error '{ErrorCode}' and the details '{ErrorDetails}'", orderRef, bankIdApiException.ErrorCode, bankIdApiException.Details);
        }

        public static void BankIdCollectFailure(this ILogger logger, string orderRef, CollectHintCode hintCode)
        {
            logger.LogInformation(BankIdLoggingEvents.BankIdCollectSoftFailure, "BankID collect failed for OrderRef '{OrderRef}' with the HintCode '{CollectHintCode}'", orderRef, hintCode);
        }

        public static void BankIdCollectPending(this ILogger logger, string orderRef, CollectHintCode hintCode)
        {
            logger.LogInformation(BankIdLoggingEvents.BankIdCollectPending, "BankID collect is pending for OrderRef '{OrderRef}' with HintCode '{CollectHintCode}'", orderRef, hintCode);
        }

        public static void BankIdCollectCompleted(this ILogger logger, string orderRef, CompletionData completionData)
        {
            logger.LogInformation(BankIdLoggingEvents.BankIdCollectCompleted, "BankID collect is completed for OrderRef '{OrderRef}'", orderRef);
            logger.LogTrace(BankIdLoggingEvents.BankIdCollectCompleted, "BankID collect is completed for OrderRef '{OrderRef}' with User (PersonalIdentityNumber: '{UserPersonalIdentityNumber}'; GivenName: '{UserGivenName}'; Surname: '{UserSurname}'; Name: '{UserName}'), Signature '{Signature}' and OcspResponse '{OcspResponse}'", orderRef, completionData.User.PersonalIdentityNumber, completionData.User.GivenName, completionData.User.Surname, completionData.User.Name, completionData.Signature, completionData.OcspResponse);
        }
    }
}
