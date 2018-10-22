using System;
using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public class BankIdAuthenticationDefaults
    {
        public const string CustomAuthenticationScheme = "bankid";
        public const string CustomDisplayName = "BankID";
        public static readonly PathString CustomCallpackPath = new PathString("/signin-bankid");

        public const string SameDeviceAuthenticationScheme = "bankid-samedevice";
        public const string SameDeviceDisplayName = "BankID (Same device)";
        public static readonly PathString SameDeviceCallpackPath = new PathString("/signin-bankid-samedevice");

        public const string OtherDeviceAuthenticationScheme = "bankid-otherdevice";
        public const string OtherDeviceDisplayName = "BankID (Other device)";
        public static readonly PathString OtherDeviceCallpackPath = new PathString("/signin-bankid-otherdevice");

        public const string IdentityProviderName = "BankID";
        public const string AuthenticationMethodName = "bankid";

        public const int StatusRefreshIntervalMs = 2000;

        public static readonly TimeSpan MaximumSessionLifespan = TimeSpan.FromHours(1);

        public const string ResourcesPath = "Resources";
    }
}