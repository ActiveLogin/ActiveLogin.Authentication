using Microsoft.Extensions.Logging;
namespace ActiveLogin.Authentication.BankId.AspNetCore.Events
{
    internal static class BankIdEventTypeNames
    {
        // BankId Authentication Handler
        public static readonly string BankIdAuthenticationTicketCreated = nameof(BankIdAuthenticationTicketCreated);

        // BankId API - Auth
        public static readonly string BankIdAuthSuccess = nameof(BankIdAuthSuccess);
        public static readonly string BankIdAuthHardFailure = nameof(BankIdAuthHardFailure);

        // BankId API - Collect
        public static readonly string BankIdCollectSoftFailure = nameof(BankIdCollectSoftFailure);
        public static readonly string BankIdCollectPending = nameof(BankIdCollectPending);
        public static readonly string BankIdCollectCompleted = nameof(BankIdCollectCompleted);
        public static readonly string BankIdCollectHardFailure = nameof(BankIdCollectHardFailure);

        // BankId API - Cancel
        public static readonly string BankIdCancelSuccess = nameof(BankIdCancelSuccess);
        public static readonly string BankIdCancelFailure = nameof(BankIdCancelFailure);
    }
}
