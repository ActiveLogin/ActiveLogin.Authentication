namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public class BankIdAuthenticationConstants
    {
        internal const string AreaName = "BankIdAuthentication";

        internal const int MinimumRefreshIntervalMs = 1000;

        internal const string InvalidReturnUrlErrorMessage = "Invalid returnUrl. Needs to be a local url.";

        public const string PersonalIdentityNumberScopeName = "personalidentitynumber";
    }
}