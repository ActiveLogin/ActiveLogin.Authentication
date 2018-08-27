using Microsoft.Extensions.Logging;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public static class GrandIdLoggingEvents
    {
       
        // GrandId API - Auth
        public static readonly EventId GrandIdAuthSuccess = new EventId(1_1_1, nameof(GrandIdAuthSuccess));
        public static readonly EventId GrandIdAuthHardFailure = new EventId(1_1_2, nameof(GrandIdAuthHardFailure));
        
    }
}