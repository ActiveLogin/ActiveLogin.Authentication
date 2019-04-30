using System;
using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public static class GrandIdAuthenticationDefaults
    {
        public const string IdentityProviderName = "GrandID";
        public static readonly TimeSpan MaximumSessionLifespan = TimeSpan.FromHours(1);

        // BankID
        public const string BankIdSameDeviceAuthenticationScheme = "grandid-bankid-samedevice";
        public const string BankIdSameDeviceDisplayName = "BankID (Denna enhet)";
        public static readonly PathString BankIdSameDeviceCallpackPath = new PathString("/signin-grandid-bankid-samedevice");

        public const string BankIdOtherDeviceAuthenticationScheme = "grandid-bankid-otherdevice";
        public const string BankIdOtherDeviceDisplayName = "BankID (Annan enhet)";
        public static readonly PathString BankIdOtherDeviceCallpackPath = new PathString("/signin-grandid-bankid-otherdevice");

        public const string BankIdChooseDeviceAuthenticationScheme = "grandid-bankid-choosedevice";
        public const string BankIdChooseDeviceDisplayName = "BankID";
        public static readonly PathString BankIdChooseDeviceCallpackPath = new PathString("/signin-grandid-bankid-choosedevice");

        public const string BankIdAuthenticationMethodName = "grandid-bankid";
    }
}