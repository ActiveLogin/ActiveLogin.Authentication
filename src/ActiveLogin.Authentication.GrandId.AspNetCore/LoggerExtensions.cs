using Microsoft.Extensions.Logging;
using System;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    internal static class LoggerExtensions
    {
        public static void GrandIdBankIdGetSessionSuccess(this ILogger logger, string sessionId)
        {
            logger.LogInformation(GrandIdLoggingEvents.GrandIdBankIdGetSessionSuccess, "GrandId (BankId) get session succedded for the sessionId '{SessionId}'", sessionId);
        }

        public static void GrandIdBankIdGetSessionFailure(this ILogger logger, string sessionId, Exception exception)
        {
            logger.LogError(GrandIdLoggingEvents.GrandIdBankIdGetSessionHardFailure, exception, "GrandId (BankId) get session failed for the sessionId '{SessionId}'", sessionId, exception);
        }

        public static void GrandIdBankIdFederatedLoginSuccess(this ILogger logger, string returnUrl, string sessionId)
        {
            logger.LogInformation(GrandIdLoggingEvents.GrandIdBankIdFederatedLoginSuccess, "GrandId (BankId) federated login succeeded for the sessionId '{SessionId}' with the returnUrl '{returnUrl}'", sessionId, returnUrl);
        }

        public static void GrandIdBankIdFederatedLoginFailure(this ILogger logger, string returnUrl, Exception exception)
        {
            logger.LogError(GrandIdLoggingEvents.GrandIdBankIdFederatedLoginHardFailure, exception, "GrandId (BankId) federated login failed with the returnUrl '{returnUrl}'", returnUrl);
        }
    }
}
