using System;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public class BankIdAuthenticationDefaults
    {
        public const string AuthenticationScheme = "bankid";
        public const string DisplayName = "BankID";

        public const string IdentityProviderName = "BankID";
        public const string AuthenticationMethodName = "bankid";

        public const int StatusRefreshIntervalMs = 2000;

        public static readonly TimeSpan MaximumSessionLifespan = TimeSpan.FromHours(1);
    }
}