using ActiveLogin.Identity.Swedish;

using Microsoft.AspNetCore.Authentication;

namespace ActiveLogin.Authentication.BankId.AspNetCore.ClaimsTransformation;

public class BankIdDefaultClaimsTransformer : IBankIdClaimsTransformer
{
    public ISystemClock Clock { get; }

    public BankIdDefaultClaimsTransformer(ISystemClock clock)
    {
        Clock = clock;
    }

    public Task TransformClaims(BankIdClaimsTransformationContext context)
    {
        AddProfileClaims(context);
        AddOptionalClaims(context);

        return Task.CompletedTask;
    }

    private Task AddProfileClaims(BankIdClaimsTransformationContext context)
    {
        var personalIdentityNumber = PersonalIdentityNumber.Parse(context.PersonalIdentityNumber);

        context.AddClaim(BankIdClaimTypes.Subject, personalIdentityNumber.To12DigitString());

        context.AddClaim(BankIdClaimTypes.Name, context.Name);
        context.AddClaim(BankIdClaimTypes.FamilyName, context.Surname);
        context.AddClaim(BankIdClaimTypes.GivenName, context.GivenName);

        context.AddClaim(BankIdClaimTypes.SwedishPersonalIdentityNumber, personalIdentityNumber.To10DigitString());

        return Task.CompletedTask;
    }

    private Task AddOptionalClaims(BankIdClaimsTransformationContext context)
    {
        if (context.BankIdOptions.TokenExpiresIn.HasValue)
        {
            var expiresUtc = Clock.UtcNow.Add(context.BankIdOptions.TokenExpiresIn.Value);
            context.AddClaim(BankIdClaimTypes.Expires, GetJwtExpires(expiresUtc));
        }

        if (context.BankIdOptions.IssueAuthenticationMethodClaim)
        {
            context.AddClaim(BankIdClaimTypes.AuthenticationMethod, context.BankIdOptions.AuthenticationMethodName);
        }

        if (context.BankIdOptions.IssueIdentityProviderClaim)
        {
            context.AddClaim(BankIdClaimTypes.IdentityProvider, context.BankIdOptions.IdentityProviderName);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Specified in: https://openid.net/specs/openid-connect-core-1_0.html#rfc.section.2
    /// </summary>
    /// <param name="expiresUtc"></param>
    /// <returns></returns>
    private static string GetJwtExpires(DateTimeOffset expiresUtc)
    {
        return expiresUtc.ToUnixTimeSeconds().ToString("D");
    }
}
