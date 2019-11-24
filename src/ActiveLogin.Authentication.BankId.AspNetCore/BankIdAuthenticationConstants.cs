namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public static class BankIdAuthenticationConstants
    {
        internal const string ProductName = "ActiveLogin-BankId-AspNetCore";

        internal const string AreaName = "BankIdAuthentication";

        internal const string InvalidReturnUrlErrorMessage = "Invalid returnUrl. Needs to be a local url.";
        
        internal const string InvalidCancelReturnUrlErrorMessage = "Invalid cancelReturnUrl. Needs to be a local url.";

        public const string AuthenticationPropertyItemSwedishPersonalIdentityNumber = "swedishPersonalIdentityNumber";
    }
}