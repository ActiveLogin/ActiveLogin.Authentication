namespace ActiveLogin.Authentication.BankId.AspNetCore.Events.Infrastructure
{
    internal static class BankIdEventTypes
    {
        // Event prefixes

        public static readonly int EventIdBase = 1000;
        public static readonly string EventNamePrefix = "ActiveLogin_";

        // ASP.NET Authentication Handler

        public static readonly int BankIdAspNetAuthenticateSuccessEventId = EventIdBase + 1_1_1;
        public static readonly string BankIdAspNetAuthenticateSuccessEventName = EventNamePrefix + "BankIdAspNetAuthenticateSuccess";

        public static readonly int BankIdAspNetAuthenticateErrorEventId = EventIdBase + 1_1_2;
        public static readonly string BankIdAspNetAuthenticateFailureEventName = EventNamePrefix + "BankIdAspNetAuthenticateFailure";

        public static readonly int BankIdAspNetChallengeSuccessEventId = EventIdBase + 1_2_1;
        public static readonly string BankIdAspNetChallengeSuccessEventName = EventNamePrefix + "BankIdAspNetChallengeSuccess";

        // BankId API - Auth

        public static readonly int BankIdAuthSuccessId = EventIdBase + 2_1_1;
        public static readonly string BankIdAuthSuccessName = EventNamePrefix + "BankIdAuthSuccess";

        public static readonly int BankIdAuthErrorEventId = EventIdBase + 2_1_2;
        public static readonly string BankIdAuthErrorEventName = EventNamePrefix + "BankIdAuthError";

        // BankId API - Collect

        public static readonly int BankIdCollectPendingId = EventIdBase + 2_2_1;
        public static readonly string BankIdCollectPendingName = EventNamePrefix + "BankIdCollectPending";

        public static readonly int BankIdCollectCompletedId = EventIdBase + 2_2_2;
        public static readonly string BankIdCollectCompletedName = EventNamePrefix + "BankIdCollectCompleted";

        public static readonly int BankIdCollectFailureId = EventIdBase + 2_2_3;
        public static readonly string BankIdCollectFailureName = EventNamePrefix + "BankIdCollectFailure";

        public static readonly int BankIdCollectErrorId = EventIdBase + 2_2_4;
        public static readonly string BankIdCollectErrorName = EventNamePrefix + "BankIdCollectError";

        // BankId API - Cancel

        public static readonly int BankIdCancelSuccessId = EventIdBase + 2_3_1;
        public static readonly string BankIdCancelSuccessName = EventNamePrefix + "BankIdCancelSuccess";

        public static readonly int BankIdCancelFailureId = EventIdBase + 2_3_2;
        public static readonly string BankIdCancelFailureName = EventNamePrefix + "BankIdCancelFailure";
    }
}
