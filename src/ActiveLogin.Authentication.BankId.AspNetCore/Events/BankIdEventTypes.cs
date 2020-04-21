namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    internal static class BankIdEventTypes
    {
        // BankId Authentication Handler
        public static readonly int BankIdAuthenticationTicketCreatedId = 1_1_1;
        public static readonly string BankIdAuthenticationTicketCreatedName = "BankIdAuthenticationTicketCreated";

        // BankId API - Auth
        public static readonly int BankIdAuthSuccessId = 2_1_1;
        public static readonly string BankIdAuthSuccessName = "BankIdAuthSuccess";

        public static readonly int BankIdAuthHardFailureId = 2_1_2;
        public static readonly string BankIdAuthHardFailureName = "BankIdAuthHardFailure";

        // BankId API - Collect
        public static readonly int BankIdCollectSoftFailureId = 2_2_2;
        public static readonly string BankIdCollectSoftFailureName = "BankIdCollectSoftFailure";

        public static readonly int BankIdCollectPendingId = 2_2_3;
        public static readonly string BankIdCollectPendingName = "BankIdCollectPending";

        public static readonly int BankIdCollectCompletedId = 2_2_4;
        public static readonly string BankIdCollectCompletedName = "BankIdCollectCompleted";

        public static readonly int BankIdCollectHardFailureId = 2_2_5;
        public static readonly string BankIdCollectHardFailureName = "BankIdCollectHardFailure";

        // BankId API - Cancel
        public static readonly int BankIdCancelSuccessId = 2_3_1;
        public static readonly string BankIdCancelSuccessName = "BankIdCancelSuccess";

        public static readonly int BankIdCancelFailureId = 2_3_2;
        public static readonly string BankIdCancelFailureName = "BankIdCancelFailure";
    }
}
