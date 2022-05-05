using System.Text.Json;

namespace ActiveLogin.Authentication.BankId.AspNetCore;

internal static class BankIdConstants
{
    internal const string ProductName = "ActiveLogin-BankId-AspNetCore";


    internal const string AreaName = "BankIdAuthentication";

    internal const string InvalidReturnUrlErrorMessage = "Invalid returnUrl. Needs to be a local url.";


    internal static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
}
