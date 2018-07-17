using ActiveLogin.Authentication.BankId.Api;
using ActiveLogin.Authentication.BankId.Api.Models;
using ActiveLogin.Identity.Swedish;
using Microsoft.Extensions.Logging;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public static class LoggingEvents
    {
        // BankId Authentication Handler
        public const int AuthenticationTicketCreated = 10_001;

        // BankId API - Auth
        public const int AuthSuccess = 20_101;
        public const int AuthHardFailure = 20_102;

        // BankId API - Collect
        public const int CollectHardFailure = 20_201;
        public const int CollectSoftFailure = 20_202;
        public const int CollectPending = 20_203;
        public const int CollectCompleted = 20_204;

    }

    public static class LoggerExtensions
    {
        // BankId Authentication Handler

        public static void BankIdAuthenticationTicketCreated(this ILogger logger, string personalIdentityNumber)
        {
            logger.LogInformation(LoggingEvents.AuthenticationTicketCreated, "BankID authentication ticket created");
            logger.LogTrace(LoggingEvents.AuthenticationTicketCreated, "BankID authentication ticket created for PersonalIdentityNumber '{PersonalIdentityNumber}'", personalIdentityNumber);
        }

        // BankID API - Auth

        public static void BankIdAuthFailure(this ILogger logger, SwedishPersonalIdentityNumber personalIdentityNumber, BankIdApiException bankIdApiException)
        {
            logger.LogError(LoggingEvents.AuthHardFailure, "BankID auth failed with the error '{ErrorCode}' and the details '{ErrorDetails}'", bankIdApiException.ErrorCode, bankIdApiException.Details);
            logger.LogTrace(LoggingEvents.AuthHardFailure, "BankID auth failed for PersonalIdentityNumber '{PersonalIdentityNumber}' with the error '{ErrorCode}' and the details '{ErrorDetails}'", personalIdentityNumber.ToLongString(), bankIdApiException.ErrorCode, bankIdApiException.Details);
        }

        public static void BankIdAuthSuccess(this ILogger logger, SwedishPersonalIdentityNumber personalIdentityNumber, string orderRef)
        {
            logger.LogInformation(LoggingEvents.AuthSuccess, "BankID auth succedded");
            logger.LogTrace(LoggingEvents.AuthSuccess, "BankID auth succedded for PersonalIdentityNumber '{PersonalIdentityNumber}' with the OrderRef '{OrderRef}'", personalIdentityNumber.ToLongString(), orderRef);
        }

        // BankID API - Collect

        public static void BankIdCollectFailure(this ILogger logger, string orderRef, BankIdApiException bankIdApiException)
        {
            logger.LogError(LoggingEvents.CollectHardFailure, "BankID collect failed with the error '{ErrorCode}' and the details '{ErrorDetails}'", bankIdApiException.ErrorCode, bankIdApiException.Details);
            logger.LogTrace(LoggingEvents.CollectHardFailure, "BankID collect failed for OrderRef '{OrderRef}' with the error '{ErrorCode}' and the details '{ErrorDetails}'", orderRef, bankIdApiException.ErrorCode, bankIdApiException.Details);
        }

        public static void BankIdCollectFailure(this ILogger logger, string orderRef, CollectHintCode hintCode)
        {
            logger.LogWarning(LoggingEvents.CollectSoftFailure, "BankID collect failed with the HintCode '{CollectHintCode}'", hintCode);
            logger.LogTrace(LoggingEvents.CollectSoftFailure, "BankID collect failed for OrderRef '{OrderRef}' with the HintCode '{CollectHintCode}'", orderRef, hintCode);
        }

        public static void BankIdCollectPending(this ILogger logger, string orderRef, CollectHintCode hintCode)
        {
            logger.LogInformation(LoggingEvents.CollectPending, "BankID collect is pending with and HintCode '{CollectHintCode}'", hintCode);
            logger.LogTrace(LoggingEvents.CollectPending, "BankID collect is pending for OrderRef '{OrderRef}' with HintCode '{CollectHintCode}'", orderRef, hintCode);
        }

        public static void BankIdCollectCompleted(this ILogger logger, string orderRef, CompletionData completionData)
        {
            logger.LogInformation(LoggingEvents.CollectCompleted, "BankID collect is completed");
            logger.LogTrace(LoggingEvents.CollectCompleted, "BankID collect is completed for OrderRef '{OrderRef}' with User (PersonalIdentityNumber: '{UserPersonalIdentityNumber}'; GivenName: '{UserGivenName}'; Surname: '{UserSurname}'; Name: '{UserName}'), Signature '{Signature}' and OcspResponse '{OcspResponse}'", orderRef, completionData.User.PersonalIdentityNumber, completionData.User.GivenName, completionData.User.Surname, completionData.User.Name, completionData.Signature, completionData.OcspResponse);
        }
    }
}
