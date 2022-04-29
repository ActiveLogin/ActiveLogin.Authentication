using System.Security.Claims;

namespace ActiveLogin.Authentication.BankId.AspNetCore.ClaimsTransformation;

public static class BankIdClaimsTransformationContextExtensions
{
    public static void AddClaim(this BankIdClaimsTransformationContext context, Claim claim)
    {
        context.Claims.Add(claim);
    }

    public static void AddClaim(this BankIdClaimsTransformationContext context, string type, string value)
    {
        context.Claims.Add(new Claim(type, value));
    }
}
