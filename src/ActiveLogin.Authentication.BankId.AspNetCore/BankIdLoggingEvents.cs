﻿using Microsoft.Extensions.Logging;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    internal static class BankIdLoggingEvents
    {
        // BankId Authentication Handler
        public static readonly EventId BankIdAuthenticationTicketCreated = new EventId(1_1_1, nameof(BankIdAuthenticationTicketCreated));

        // BankId API - Auth
        public static readonly EventId BankIdAuthSuccess = new EventId(2_1_1, nameof(BankIdAuthSuccess));
        public static readonly EventId BankIdAuthHardFailure = new EventId(2_1_2, nameof(BankIdAuthHardFailure));
        public static readonly EventId BankIdAuthCancel = new EventId(2_1_3, nameof(BankIdAuthCancel));

        // BankId API - Collect
        public static readonly EventId BankIdCollectSoftFailure = new EventId(2_2_2, nameof(BankIdCollectSoftFailure));
        public static readonly EventId BankIdCollectPending = new EventId(2_2_3, nameof(BankIdCollectPending));
        public static readonly EventId BankIdCollectCompleted = new EventId(2_2_4, nameof(BankIdCollectCompleted));
    }
}
