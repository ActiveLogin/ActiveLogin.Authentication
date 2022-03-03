using System.Threading.Tasks;

using ActiveLogin.Authentication.BankId.AspNetCore.Serialization;
using ActiveLogin.Identity.Swedish;
using ActiveLogin.Identity.Swedish.Extensions;

using Microsoft.AspNetCore.Authentication;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public class BankIdDefaultClaimsTransformer : IBankIdClaimsTransformer
    {
        public ISystemClock Clock { get; }

        public BankIdDefaultClaimsTransformer(ISystemClock clock)
        {
            Clock = clock;
        }

        public Task TransformClaims(BankIdClaimsTransformationContext context)
        {
            var personalIdentityNumber = PersonalIdentityNumber.Parse(context.PersonalIdentityNumber);

            AddProfileClaims(context, personalIdentityNumber);
            AddOptionalClaims(context, personalIdentityNumber);

            return Task.CompletedTask;
        }

        private Task AddProfileClaims(BankIdClaimsTransformationContext context, PersonalIdentityNumber personalIdentityNumber)
        {
            context.AddClaim(BankIdClaimTypes.Subject, personalIdentityNumber.To12DigitString());

            context.AddClaim(BankIdClaimTypes.Name, context.Name);
            context.AddClaim(BankIdClaimTypes.FamilyName, context.Surname);
            context.AddClaim(BankIdClaimTypes.GivenName, context.GivenName);

            context.AddClaim(BankIdClaimTypes.SwedishPersonalIdentityNumber, personalIdentityNumber.To10DigitString());

            return Task.CompletedTask;
        }

        private Task AddOptionalClaims(BankIdClaimsTransformationContext context, PersonalIdentityNumber personalIdentityNumber)
        {
            if (context.BankIdOptions.TokenExpiresIn.HasValue)
            {
                var expiresUtc = Clock.UtcNow.Add(context.BankIdOptions.TokenExpiresIn.Value);
                context.AddClaim(BankIdClaimTypes.Expires, JwtSerializer.GetExpires(expiresUtc));
            }

            if (context.BankIdOptions.IssueAuthenticationMethodClaim)
            {
                context.AddClaim(BankIdClaimTypes.AuthenticationMethod, context.BankIdOptions.AuthenticationMethodName);
            }

            if (context.BankIdOptions.IssueIdentityProviderClaim)
            {
                context.AddClaim(BankIdClaimTypes.IdentityProvider, context.BankIdOptions.IdentityProviderName);
            }

            if (context.BankIdOptions.IssueGenderClaim)
            {
                var jwtGender = JwtSerializer.GetGender(personalIdentityNumber.GetGenderHint());
                if (!string.IsNullOrEmpty(jwtGender))
                {
                    context.AddClaim(BankIdClaimTypes.Gender, jwtGender);
                }
            }

            if (context.BankIdOptions.IssueBirthdateClaim)
            {
                var jwtBirthdate = JwtSerializer.GetBirthdate(personalIdentityNumber.GetDateOfBirthHint());
                context.AddClaim(BankIdClaimTypes.Birthdate, jwtBirthdate);
            }

            return Task.CompletedTask;
        }
    }
}
