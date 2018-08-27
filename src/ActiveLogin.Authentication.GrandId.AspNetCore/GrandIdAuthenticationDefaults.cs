namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public class GrandIdAuthenticationDefaults
    {
        public const string AuthenticationScheme = "grandid";
        public const string DisplayName = "GrandId";

        public const string IdentityProviderName = "GrandId";
        public const string AuthenticationMethodName = "grandid";

        public const int MaximumSessionLifespanS = 3600;
    }
}