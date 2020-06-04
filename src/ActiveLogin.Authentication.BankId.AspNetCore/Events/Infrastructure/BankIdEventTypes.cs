namespace ActiveLogin.Authentication.BankId.AspNetCore.Events.Infrastructure
{
    internal static class BankIdEventTypes
    {
        // Event prefixes

        public static readonly int EventIdBase = 1000;
        public static readonly string EventNamePrefix = "ActiveLogin_BankId_";

        // ASP.NET Authentication Handler

        public static readonly int AspNetChallengeSuccessEventId = EventIdBase + 1_1_1;
        public static readonly string AspNetChallengeSuccessEventName = EventNamePrefix + "AspNetChallengeSuccess";

        public static readonly int AspNetAuthenticateSuccessEventId = EventIdBase + 1_2_1;
        public static readonly string AspNetAuthenticateSuccessEventName = EventNamePrefix + "AspNetAuthenticateSuccess";

        public static readonly int AspNetAuthenticateErrorEventId = EventIdBase + 1_2_2;
        public static readonly string AspNetAuthenticateFailureEventName = EventNamePrefix + "AspNetAuthenticateFailure";

        // BankId API - Auth

        public static readonly int AuthSuccessId = EventIdBase + 2_1_1;
        public static readonly string AuthSuccessName = EventNamePrefix + "AuthSuccess";

        public static readonly int AuthErrorEventId = EventIdBase + 2_1_2;
        public static readonly string AuthErrorEventName = EventNamePrefix + "AuthError";

        // BankId API - Collect

        public static readonly int CollectPendingId = EventIdBase + 2_2_1;
        public static readonly string CollectPendingName = EventNamePrefix + "CollectPending";

        public static readonly int CollectCompletedId = EventIdBase + 2_2_2;
        public static readonly string CollectCompletedName = EventNamePrefix + "CollectCompleted";

        public static readonly int CollectFailureId = EventIdBase + 2_2_3;
        public static readonly string CollectFailureName = EventNamePrefix + "CollectFailure";

        public static readonly int CollectErrorId = EventIdBase + 2_2_4;
        public static readonly string CollectErrorName = EventNamePrefix + "CollectError";

        // BankId API - Cancel

        public static readonly int CancelSuccessId = EventIdBase + 2_3_1;
        public static readonly string CancelSuccessName = EventNamePrefix + "CancelSuccess";

        public static readonly int CancelFailureId = EventIdBase + 2_3_2;
        public static readonly string CancelFailureName = EventNamePrefix + "CancelFailure";
    }
}
