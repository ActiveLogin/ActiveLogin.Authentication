using System.Text.Json;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public static class BankIdConstants
    {
        internal const string ProductName = "ActiveLogin-BankId-AspNetCore";

        internal const string AreaName = "BankIdAuthentication";

        internal const string InvalidReturnUrlErrorMessage = "Invalid returnUrl. Needs to be a local url.";

        internal const string BankIdApiVersion = "5.1";

        internal static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        internal const int MaxRetryLoginAttempts = 5;

        public const string AuthenticationPropertyItemSwedishPersonalIdentityNumber = "swedishPersonalIdentityNumber";

        internal const string UserAgentHttpHeaderName = "User-Agent";
    }
}
