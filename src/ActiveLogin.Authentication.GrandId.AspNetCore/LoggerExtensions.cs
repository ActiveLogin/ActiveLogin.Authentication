using Microsoft.Extensions.Logging;
using System;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    internal static class LoggerExtensions
    {
        public static void GrandIdGetSessionSuccess(this ILogger logger, string sessionId)
        {
            logger.LogInformation(GrandIdLoggingEvents.GrandIdGetSessionSuccess, "GrandId get session succedded for the sessionId '{SessionId}'", sessionId);
        }

        public static void GrandIdGetSessionFailure(this ILogger logger, string sessionId, Exception exception)
        {
            logger.LogError(GrandIdLoggingEvents.GrandIdGetSessionHardFailure, exception, "GrandId get session failed for the sessionId '{SessionId}'", sessionId, exception);
        }

        public static void GrandIdFederatedLoginSuccess(this ILogger logger, string authenticateServiceKey, string returnUrl, string sessionId)
        {
            logger.LogInformation(GrandIdLoggingEvents.GrandIdFederatedLoginSuccess, "GrandId federated login succedded for authenticateServiceKey '{AuthenticateServiceKey}' and returnUrl '{returnUrl}'", authenticateServiceKey, returnUrl);
        }

        public static void GrandIdFederatedLoginFailure(this ILogger logger, string authenticateServiceKey, string returnUrl, Exception exception)
        {
            logger.LogError(GrandIdLoggingEvents.GrandIdFederatedLoginHardFailure, exception, "GrandId federated login failed for authenticateServiceKey '{AuthenticateServiceKey}' and returnUrl '{returnUrl}'", authenticateServiceKey, returnUrl);
        }
    }
}
