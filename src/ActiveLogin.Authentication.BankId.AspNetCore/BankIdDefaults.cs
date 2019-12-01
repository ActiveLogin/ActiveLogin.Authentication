using System;
using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public static class BankIdDefaults
    {
        public const string IdentityProviderName = "BankID";
        public const string AuthenticationMethodName = "bankid";
        public static readonly TimeSpan MaximumSessionLifespan = TimeSpan.FromHours(1);

        public const int StatusRefreshIntervalMs = 2000;
        public const string ResourcesPath = "Resources";


        public const string SameDeviceAuthenticationScheme = "bankid-samedevice";
        public const string SameDeviceDisplayName = "BankID (Denna enhet)";
        public static readonly PathString SameDeviceCallbackPath = new PathString("/signin-bankid-samedevice");

        public const string OtherDeviceAuthenticationScheme = "bankid-otherdevice";
        public const string OtherDeviceDisplayName = "BankID (Annan enhet)";
        public static readonly PathString OtherDeviceCallbackPath = new PathString("/signin-bankid-otherdevice");
    }
}