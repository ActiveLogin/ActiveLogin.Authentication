using Microsoft.Extensions.Logging;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    internal static class GrandIdLoggingEvents
    {
        // GrandId API - Auth
        public static readonly EventId GrandIdFederatedLoginSuccess = new EventId(1_1_1, nameof(GrandIdFederatedLoginSuccess));
        public static readonly EventId GrandIdFederatedLoginHardFailure = new EventId(1_1_2, nameof(GrandIdFederatedLoginHardFailure));

        // GrandId API - Get session
        public static readonly EventId GrandIdGetSessionSuccess = new EventId(2_1_1, nameof(GrandIdGetSessionSuccess));
        public static readonly EventId GrandIdGetSessionHardFailure = new EventId(2_1_2, nameof(GrandIdGetSessionHardFailure));
    }
}