using System;
using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public class BankIdAuthenticationDefaults
    {
        public const string AuthenticationScheme = "bankid";
        public const string DisplayName = "BankID";
        public static readonly PathString CallpackPath = new PathString("/signin-bankid");

        public const string IdentityProviderName = "BankID";
        public const string AuthenticationMethodName = "bankid";

        public const int StatusRefreshIntervalMs = 2000;

        public static readonly TimeSpan MaximumSessionLifespan = TimeSpan.FromHours(1);

        public const string ResourcesPath = "Resources";
    }
}