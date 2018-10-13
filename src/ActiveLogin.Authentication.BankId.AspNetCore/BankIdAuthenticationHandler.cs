using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Identity.Swedish;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public class BankIdAuthenticationHandler : RemoteAuthenticationHandler<BankIdAuthenticationOptions>
    {
        private readonly ILogger<BankIdAuthenticationHandler> _logger;
        private readonly IBankIdLoginResultProtector _loginResultProtector;

        public BankIdAuthenticationHandler(
            IOptionsMonitor<BankIdAuthenticationOptions> options,
            ILoggerFactory loggerFactory,
            UrlEncoder encoder,
            ISystemClock clock,
            ILogger<BankIdAuthenticationHandler> logger,
            IBankIdLoginResultProtector loginResultProtector)
            : base(options, loggerFactory, encoder, clock)
        {
            _logger = logger;
            _loginResultProtector = loginResultProtector;
        }

        protected override Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
        {
            var state = GetStateFromCookie();
            if (state == null)
            {
                return Task.FromResult(HandleRequestResult.Fail("Invalid state cookie."));
            }

            DeleteStateCookie();

            var loginResultProtected = Request.Query["loginResult"];
            if (string.IsNullOrEmpty(loginResultProtected))
            {
                return Task.FromResult(HandleRequestResult.Fail("Missing login result."));
            }

            var loginResult = _loginResultProtector.Unprotect(loginResultProtected);
            if (loginResult == null || !loginResult.IsSuccessful)
            {
                return Task.FromResult(HandleRequestResult.Fail("Invalid login result."));
            }

            var properties = state.AuthenticationProperties;
            var ticket = GetAuthenticationTicket(loginResult, properties);

            _logger.BankIdAuthenticationTicketCreated(loginResult.PersonalIdentityNumber);

            return Task.FromResult(HandleRequestResult.Success(ticket));
        }

        private AuthenticationTicket GetAuthenticationTicket(BankIdLoginResult loginResult, AuthenticationProperties properties)
        {
            DateTimeOffset? expiresUtc = null;
            if (Options.TokenExpiresIn.HasValue)
            {
                expiresUtc = Clock.UtcNow.Add(Options.TokenExpiresIn.Value);
                properties.ExpiresUtc = expiresUtc;
            }

            var claims = GetClaims(loginResult, expiresUtc);
            var identity = new ClaimsIdentity(claims, Scheme.Name, BankIdClaimTypes.Name, BankIdClaimTypes.Role);
            var principal = new ClaimsPrincipal(identity);

            return new AuthenticationTicket(principal, properties, Scheme.Name);
        }

        private IEnumerable<Claim> GetClaims(BankIdLoginResult loginResult, DateTimeOffset? expiresUtc)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Parse(loginResult.PersonalIdentityNumber);
            var claims = new List<Claim>
            {
                new Claim(BankIdClaimTypes.Subject, personalIdentityNumber.ToLongString()),

                new Claim(BankIdClaimTypes.Name, loginResult.Name),
                new Claim(BankIdClaimTypes.FamilyName, loginResult.Surname),
                new Claim(BankIdClaimTypes.GivenName, loginResult.GivenName),

                new Claim(BankIdClaimTypes.SwedishPersonalIdentityNumber, personalIdentityNumber.ToShortString())
            };

            AddOptionalClaims(claims, personalIdentityNumber, expiresUtc);

            return claims;
        }

        private void AddOptionalClaims(List<Claim> claims, SwedishPersonalIdentityNumber personalIdentityNumber, DateTimeOffset? expiresUtc)
        {
            if (expiresUtc.HasValue)
            {
                claims.Add(new Claim(BankIdClaimTypes.Expires, expiresUtc.Value.ToUnixTimeSeconds().ToString("D")));
            }

            if (Options.IssueAuthenticationMethodClaim)
            {
                claims.Add(new Claim(BankIdClaimTypes.AuthenticationMethod, Options.AuthenticationMethodName));
            }

            if (Options.IssueIdentityProviderClaim)
            {
                claims.Add(new Claim(BankIdClaimTypes.IdentityProvider, Options.IdentityProviderName));
            }

            if (Options.IssueGenderClaim)
            {
                // Specified in: http://openid.net/specs/openid-connect-core-1_0.html#rfc.section.5.1
                var jwtGender = GetJwtGender(personalIdentityNumber.Gender);
                if (!string.IsNullOrEmpty(jwtGender))
                {
                    claims.Add(new Claim(BankIdClaimTypes.Gender, jwtGender));
                }
            }

            if (Options.IssueBirthdateClaim)
            {
                // Specified in: http://openid.net/specs/openid-connect-core-1_0.html#rfc.section.5.1
                var jwtBirthdate = GetJwtBirthdate(personalIdentityNumber.DateOfBirth);
                claims.Add(new Claim(BankIdClaimTypes.Birthdate, jwtBirthdate));
            }
        }

        private static string GetJwtGender(SwedishGender gender)
        {
            switch (gender)
            {
                case SwedishGender.Female:
                    return "female";
                case SwedishGender.Male:
                    return "male";
            }

            return string.Empty;
        }

        private static string GetJwtBirthdate(DateTime dateOfBirth)
        {
            return dateOfBirth.Date.ToString("yyyy-MM-dd");
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            AppendStateCookie(properties);

            var loginUrl = GetLoginUrl();
            Response.Redirect(loginUrl);

            return Task.CompletedTask;
        }

        private string GetLoginUrl()
        {
            return $"{Options.BankIdLoginPath}?returnUrl={UrlEncoder.Encode(Options.CallbackPath)}";
        }

        private void AppendStateCookie(AuthenticationProperties properties)
        {
            var state = new BankIdState()
            {
                AuthenticationProperties = properties
            };
            var cookieOptions = Options.StateCookie.Build(Context, Clock.UtcNow);

            Response.Cookies.Append(Options.StateCookie.Name, Options.StateDataFormat.Protect(state), cookieOptions);
        }

        private BankIdState GetStateFromCookie()
        {
            var protectedState = Request.Cookies[Options.StateCookie.Name];
            if (string.IsNullOrEmpty(protectedState))
            {
                return null;
            }

            var state = Options.StateDataFormat.Unprotect(protectedState);
            return state;
        }

        private void DeleteStateCookie()
        {
            var cookieOptions = Options.StateCookie.Build(Context, Clock.UtcNow);
            Response.Cookies.Delete(Options.StateCookie.Name, cookieOptions);
        }
    }
}