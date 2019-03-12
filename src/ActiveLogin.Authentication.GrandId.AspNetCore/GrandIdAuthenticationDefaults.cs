using System;
using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public class GrandIdAuthenticationDefaults
    {
        public const string IdentityProviderName = "GrandID";

        // BankID
        public const string BankIdSameDeviceAuthenticationScheme = "grandid-bankid-samedevice";
        public const string BankIdSameDeviceDisplayName = "BankID (Denna enhet)";

        public const string BankIdOtherDeviceAuthenticationScheme = "grandid-bankid-otherdevice";
        public const string BankIdOtherDeviceDisplayName = "BankID (Annan enhet)";

        public const string BankIdChooseDeviceAuthenticationScheme = "grandid-bankid-choosedevice";
        public const string BankIdChooseDeviceDisplayName = "BankID";

        public const string BankIdAuthenticationMethodName = "grandid-bankid";
        public static readonly TimeSpan MaximumSessionLifespan = TimeSpan.FromHours(1);

        public static readonly PathString BankIdSameDeviceCallpackPath = new PathString("/signin-grandid-bankid-samedevice");

        public static readonly PathString BankIdOtherDeviceCallpackPath = new PathString("/signin-grandid-bankid-otherdevice");

        public static readonly PathString BankIdChooseDeviceCallpackPath = new PathString("/signin-grandid-bankid-choosedevice");
    }
}
