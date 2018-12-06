using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using ActiveLogin.Authentication.GrandId.Api;
using ActiveLogin.Authentication.GrandId.Api.Models;
using ActiveLogin.Authentication.GrandId.AspNetCore.Serialization;
using ActiveLogin.Identity.Swedish;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public class GrandIdBankIdAuthenticationHandler : GrandIdAuthenticationHandler<GrandIdBankIdAuthenticationOptions, GrandIdBankIdAuthenticationHandler>
    {
        public GrandIdBankIdAuthenticationHandler(
            IOptionsMonitor<GrandIdBankIdAuthenticationOptions> options,
            ILoggerFactory loggerFactory,
            UrlEncoder encoder,
            ISystemClock clock,
            ILogger<GrandIdBankIdAuthenticationHandler> logger,
            IGrandIdApiClient grandIdApiClient
            )
            : base(options, loggerFactory, encoder, clock, logger, grandIdApiClient)
        {
        }

        protected override string GetGrandIdAuthenticateServiceKey()
        {
            return Options.GrandIdAuthenticateServiceKey;
        }

        protected override SwedishPersonalIdentityNumber GetSwedishPersonalIdentityNumber(AuthenticationProperties properties)
        {
            if (properties.Items.TryGetValue(GrandIdAuthenticationConstants.AuthenticationPropertyItemSwedishPersonalIdentityNumber, out var swedishPersonalIdentityNumber))
            {
                if (!string.IsNullOrWhiteSpace(swedishPersonalIdentityNumber))
                {
                    return SwedishPersonalIdentityNumber.Parse(swedishPersonalIdentityNumber);
                }
            }

            return null;
        }

        protected override IEnumerable<Claim> GetClaims(SessionStateResponse loginResult)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Parse(loginResult.UserAttributes.PersonalIdentityNumber);
            var claims = new List<Claim>
            {
                new Claim(GrandIdClaimTypes.Subject, personalIdentityNumber.ToLongString()),

                new Claim(GrandIdClaimTypes.Name, loginResult.UserAttributes.Name),
                new Claim(GrandIdClaimTypes.FamilyName, loginResult.UserAttributes.Surname),
                new Claim(GrandIdClaimTypes.GivenName, loginResult.UserAttributes.GivenName),

                new Claim(GrandIdClaimTypes.SwedishPersonalIdentityNumber, personalIdentityNumber.ToShortString())
            };

            if (Options.IssueGenderClaim)
            {
                var jwtGender = JwtSerializer.GetGender(personalIdentityNumber.GetGenderHint());
                if (!string.IsNullOrEmpty(jwtGender))
                {
                    claims.Add(new Claim(GrandIdClaimTypes.Gender, jwtGender));
                }
            }

            if (Options.IssueBirthdateClaim)
            {
                var jwtBirthdate = JwtSerializer.GetBirthdate(personalIdentityNumber.GetDateOfBirthHint());
                claims.Add(new Claim(GrandIdClaimTypes.Birthdate, jwtBirthdate));
            }

            return claims;
        }
    }
}