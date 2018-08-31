using Microsoft.Extensions.Logging;
using System;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public static class LoggerExtensions
    {
        public static void GrandIdAuthSuccess(this ILogger logger, string sessionId)
        {
            logger.LogInformation(GrandIdLoggingEvents.GrandIdAuthSuccess, "GrandId auth succedded for the sessionId '{SessionId}'", sessionId);
        }

        public static void GrandIdAuthFailure(this ILogger logger, string sessionId, Exception exception)
        {
            logger.LogError(GrandIdLoggingEvents.GrandIdAuthHardFailure, exception, "GrandId auth failed for the sessionId '{SessionId}'", sessionId, exception);
        }
    }
}
