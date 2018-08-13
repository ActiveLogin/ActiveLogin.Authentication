using Microsoft.Extensions.Logging;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public static class GrandIdLoggingEvents
    {
        // GrandId Authentication Handler
        public static readonly EventId GrandIdAuthenticationTicketCreated = new EventId(1_1_1, nameof(GrandIdAuthenticationTicketCreated));

        // GrandId API - Auth
        public static readonly EventId GrandIdAuthSuccess = new EventId(2_1_1, nameof(GrandIdAuthSuccess));
        public static readonly EventId GrandIdAuthHardFailure = new EventId(2_1_2, nameof(GrandIdAuthHardFailure));

        // GrandId API - Collect
        public static readonly EventId GrandIdCollectSoftFailure = new EventId(2_2_2, nameof(GrandIdCollectSoftFailure));
        public static readonly EventId GrandIdCollectPending = new EventId(2_2_3, nameof(GrandIdCollectPending));
        public static readonly EventId GrandIdCollectCompleted = new EventId(2_2_4, nameof(GrandIdCollectCompleted));
    }
}