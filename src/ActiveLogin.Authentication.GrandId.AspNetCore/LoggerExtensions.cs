using Microsoft.Extensions.Logging;
using System;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public static class LoggerExtensions
    {
        public static void GrandIdGetSessionSuccess(this ILogger logger, string sessionId)
        {
            logger.LogInformation(GrandIdLoggingEvents.GrandIdGetSessionSuccess, "GrandId get session succedded for the sessionId '{SessionId}'", sessionId);
        }

        public static void GrandIdGetSessionFailure(this ILogger logger, string sessionId, Exception exception)
        {
            logger.LogError(GrandIdLoggingEvents.GrandIdGetSessionHardFailure, exception, "GrandId get session failed for the sessionId '{SessionId}'", sessionId, exception);
        }

        public static void GrandIdAuthSuccess(this ILogger logger, string authenticateServiceKey, string returnUrl, string sessionId)
        {
            logger.LogInformation(GrandIdLoggingEvents.GrandIdAuthSuccess, "GrandId auth succedded for authenticateServiceKey '{AuthenticateServiceKey}' and returnUrl '{returnUrl}'", authenticateServiceKey, returnUrl);
        }

        public static void GrandIdAuthFailure(this ILogger logger, string authenticateServiceKey, string returnUrl, Exception exception)
        {
            logger.LogError(GrandIdLoggingEvents.GrandIdAuthHardFailure, exception, "GrandId auth failed for authenticateServiceKey '{AuthenticateServiceKey}' and returnUrl '{returnUrl}'", authenticateServiceKey, returnUrl);
        }
    }
}
