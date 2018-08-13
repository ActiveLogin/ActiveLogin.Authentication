using ActiveLogin.Authentication.GrandId.Api;
using ActiveLogin.Authentication.GrandId.Api.Models;
using ActiveLogin.Identity.Swedish;
using Microsoft.Extensions.Logging;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public static class LoggerExtensions
    {
        // GrandId Authentication Handler

        public static void GrandIdAuthenticationTicketCreated(this ILogger logger, string personalIdentityNumber)
        {
            logger.LogInformation(GrandIdLoggingEvents.GrandIdAuthenticationTicketCreated, "BankID authentication ticket created");
            logger.LogTrace(GrandIdLoggingEvents.GrandIdAuthenticationTicketCreated, "BankID authentication ticket created for PersonalIdentityNumber '{PersonalIdentityNumber}'", personalIdentityNumber);
        }

        // BankID API - Auth

        public static void GrandIdAuthFailure(this ILogger logger, SwedishPersonalIdentityNumber personalIdentityNumber, GrandIdApiException bankIdApiException)
        {
            logger.LogError(bankIdApiException, "BankID auth failed with the error '{ErrorCode}' and the details '{ErrorDetails}'", bankIdApiException.ErrorCode, bankIdApiException.Details);
            logger.LogTrace(GrandIdLoggingEvents.GrandIdAuthHardFailure, "BankID auth failed for PersonalIdentityNumber '{PersonalIdentityNumber}' with the error '{ErrorCode}' and the details '{ErrorDetails}'", personalIdentityNumber.ToLongString(), bankIdApiException.ErrorCode, bankIdApiException.Details);
        }

        public static void GrandIdAuthSuccess(this ILogger logger, SwedishPersonalIdentityNumber personalIdentityNumber, string orderRef)
        {
            logger.LogInformation(GrandIdLoggingEvents.GrandIdAuthSuccess, "BankID auth succedded with the OrderRef '{OrderRef}'", orderRef);
            logger.LogTrace(GrandIdLoggingEvents.GrandIdAuthSuccess, "BankID auth succedded for PersonalIdentityNumber '{PersonalIdentityNumber}' with the OrderRef '{OrderRef}'", personalIdentityNumber.ToLongString(), orderRef);
        }

        // BankID API - Collect

        public static void GrandIdCollectFailure(this ILogger logger, string orderRef, GrandIdApiException bankIdApiException)
        {
            logger.LogError(bankIdApiException, "BankID collect failed for OrderRef '{OrderRef}' with the error '{ErrorCode}' and the details '{ErrorDetails}'", orderRef, bankIdApiException.ErrorCode, bankIdApiException.Details);
        }

        public static void GrandIdCollectFailure(this ILogger logger, string orderRef, CollectHintCode hintCode)
        {
            logger.LogInformation(GrandIdLoggingEvents.GrandIdCollectSoftFailure, "BankID collect failed for OrderRef '{OrderRef}' with the HintCode '{CollectHintCode}'", orderRef, hintCode);
        }

        public static void GrandIdCollectPending(this ILogger logger, string orderRef, CollectHintCode hintCode)
        {
            logger.LogInformation(GrandIdLoggingEvents.GrandIdCollectPending, "BankID collect is pending for OrderRef '{OrderRef}' with HintCode '{CollectHintCode}'", orderRef, hintCode);
        }

        public static void GrandIdCollectCompleted(this ILogger logger, string orderRef, CompletionData completionData)
        {
            logger.LogInformation(GrandIdLoggingEvents.GrandIdCollectCompleted, "BankID collect is completed for OrderRef '{OrderRef}'", orderRef);
            logger.LogTrace(GrandIdLoggingEvents.GrandIdCollectCompleted, "BankID collect is completed for OrderRef '{OrderRef}' with User (PersonalIdentityNumber: '{UserPersonalIdentityNumber}'; GivenName: '{UserGivenName}'; Surname: '{UserSurname}'; Name: '{UserName}'), Signature '{Signature}' and OcspResponse '{OcspResponse}'", orderRef, completionData.User.PersonalIdentityNumber, completionData.User.GivenName, completionData.User.Surname, completionData.User.Name, completionData.Signature, completionData.OcspResponse);
        }
    }
}
