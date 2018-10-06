using System;
using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public class GrandIdAuthenticationDefaults
    {
        public const string SameDeviceAuthenticationScheme = "grandid-samedevice";
        public static readonly PathString SameDeviceCallpackPath = new PathString("/signin-grandid-samedevice");

        public const string OtherDeviceAuthenticationScheme = "grandid-otherdevice";
        public static readonly PathString OtherDeviceCallpackPath = new PathString("/signin-grandid-otherdevice");

        public const string ChooseDeviceAuthenticationScheme = "grandid-choosedevice";
        public static readonly PathString ChooseDeviceCallpackPath = new PathString("/signin-grandid-choosedevice");

        public const string IdentityProviderName = "GrandId";
        public const string AuthenticationMethodName = "grandid";

        public static readonly TimeSpan MaximumSessionLifespan = TimeSpan.FromHours(1);
    }
}