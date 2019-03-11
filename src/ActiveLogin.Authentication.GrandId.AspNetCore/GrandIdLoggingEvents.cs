using Microsoft.Extensions.Logging;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    internal static class GrandIdLoggingEvents
    {
        // GrandId - BankId - Federated Login
        public static readonly EventId GrandIdBankIdFederatedLoginSuccess =
            new EventId(1_1_1, nameof(GrandIdBankIdFederatedLoginSuccess));

        public static readonly EventId GrandIdBankIdFederatedLoginHardFailure =
            new EventId(1_1_2, nameof(GrandIdBankIdFederatedLoginHardFailure));

        // GrandId - BankId - Get session
        public static readonly EventId GrandIdBankIdGetSessionSuccess =
            new EventId(2_1_1, nameof(GrandIdBankIdGetSessionSuccess));

        public static readonly EventId GrandIdBankIdGetSessionHardFailure =
            new EventId(2_1_2, nameof(GrandIdBankIdGetSessionHardFailure));
    }
}
