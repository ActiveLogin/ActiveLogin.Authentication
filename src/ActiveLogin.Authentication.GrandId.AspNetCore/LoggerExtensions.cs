using Microsoft.Extensions.Logging;
using System;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public static class LoggerExtensions
    {

        public static void GrandIdAuthFailure(this ILogger logger, string errorMessage)
        {
            logger.LogError(errorMessage);
        }

        public static void GrandIdAuthSuccess(this ILogger logger, string sessionId)
        {
            logger.LogInformation(GrandIdLoggingEvents.GrandIdAuthSuccess, "GrandId auth succedded for the sessionId '{SessionId}'", sessionId);
            logger.LogTrace(GrandIdLoggingEvents.GrandIdAuthSuccess, "GrandId auth succedded for sessionId '{SessionId}' ", sessionId);
        }

        public static void GrandIdAuthFailed(this ILogger logger, string sessionId, Exception exception)
        {
            logger.LogInformation(GrandIdLoggingEvents.GrandIdAuthHardFailure, "GrandId auth failed for the sessionId '{SessionId}'", sessionId, exception);
            logger.LogTrace(GrandIdLoggingEvents.GrandIdAuthHardFailure, "GrandId auth failed for sessionId '{SessionId}' ", sessionId, exception);
        }
    }
}
