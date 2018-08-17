using ActiveLogin.Authentication.GrandId.Api;
using ActiveLogin.Authentication.GrandId.Api.Models;
using ActiveLogin.Identity.Swedish;
using Microsoft.Extensions.Logging;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public static class LoggerExtensions
    {
        // GrandId Authentication Handler

        //public static void GrandIdAuthenticationTicketCreated(this ILogger logger, string personalIdentityNumber)
        //{
        //    logger.LogInformation(GrandIdLoggingEvents.GrandIdAuthenticationTicketCreated, "BankID authentication ticket created");
        //    logger.LogTrace(GrandIdLoggingEvents.GrandIdAuthenticationTicketCreated, "BankID authentication ticket created for PersonalIdentityNumber '{PersonalIdentityNumber}'", personalIdentityNumber);
        //}

        // GrandId API - Auth

        public static void GrandIdAuthFailure(this ILogger logger, string errorMessage)
        {
            logger.LogError(errorMessage);
        }

        public static void GrandIdAuthSuccess(this ILogger logger, string message, string sessionId)
        {
            //logger.LogInformation(GrandIdLoggingEvents.GrandIdAuthSuccess, "GrandId auth succedded with the sessionId '{SessionId}'", sessionId);
            //logger.LogTrace(GrandIdLoggingEvents.GrandIdAuthSuccess, "GrandId auth succedded for sessionId '{SessionId}' ", sessionId);
        }
    }
}
