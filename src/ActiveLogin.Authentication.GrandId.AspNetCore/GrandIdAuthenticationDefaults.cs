using System;
using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public class GrandIdAuthenticationDefaults
    {
        public const string SameDeviceAuthenticationScheme = "grandid-samedevice";
        public const string SameDeviceDisplayName = "BankID (Denna enhet)";
        public static readonly PathString SameDeviceCallpackPath = new PathString("/signin-grandid-samedevice");

        public const string OtherDeviceAuthenticationScheme = "grandid-otherdevice";
        public const string OtherDeviceDisplayName = "BankID (Annan enhet)";
        public static readonly PathString OtherDeviceCallpackPath = new PathString("/signin-grandid-otherdevice");

        public const string ChooseDeviceAuthenticationScheme = "grandid-choosedevice";
        public const string ChooseDeviceDisplayName = "BankID";
        public static readonly PathString ChooseDeviceCallpackPath = new PathString("/signin-grandid-choosedevice");

        public const string SithsAuthenticationScheme = "grandid-siths";
        public const string SithsDisplayName = "Siths";
        public static readonly PathString SithsCallpackPath = new PathString("/signin-grandid-siths");

        public const string IdentityProviderName = "GrandId";
        public const string AuthenticationMethodName = "grandid";

        public static readonly TimeSpan MaximumSessionLifespan = TimeSpan.FromHours(1);
    }
}