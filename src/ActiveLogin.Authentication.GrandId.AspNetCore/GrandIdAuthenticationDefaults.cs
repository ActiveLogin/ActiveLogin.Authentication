﻿namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public class GrandIdAuthenticationDefaults
    {
        public const string AuthenticationScheme = "bankid";
        public const string DisplayName = "BankID";

        public const string IdentityProviderName = "BankID";
        public const string AuthenticationMethodName = "bankid";

        public const int StatusRefreshIntervalMs = 2000;

        public const int MaximumSessionLifespanS = 3600;
    }
}