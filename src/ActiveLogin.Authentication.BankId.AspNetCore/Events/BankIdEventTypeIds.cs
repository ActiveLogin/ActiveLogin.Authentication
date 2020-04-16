namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    internal static class BankIdEventTypeIds
    {
        // BankId Authentication Handler
        public static readonly int BankIdAuthenticationTicketCreated = 1_1_1;

        // BankId API - Auth
        public static readonly int BankIdAuthSuccess = 2_1_1;
        public static readonly int BankIdAuthHardFailure = 2_1_2;

        // BankId API - Collect
        public static readonly int BankIdCollectSoftFailure = 2_2_2;
        public static readonly int BankIdCollectPending = 2_2_3;
        public static readonly int BankIdCollectCompleted = 2_2_4;
        public static readonly int BankIdCollectHardFailure = 2_2_5;

        // BankId API - Cancel
        public static readonly int BankIdCancelSuccess = 2_3_1;
        public static readonly int BankIdCancelFailure = 2_3_2;
    }
}
