using System;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public class GrandIdAuthenticationDefaults
    {
        public const string AuthenticationScheme = "grandid";
        public const string DisplayName = "GrandId";

        public const string IdentityProviderName = "GrandId";
        public const string AuthenticationMethodName = "grandid";

        public static readonly TimeSpan MaximumSessionLifespan = TimeSpan.FromHours(1);
    }
}