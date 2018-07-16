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
        private readonly IBankIdLoginResultProtector _loginResultProtector;

        public BankIdAuthenticationHandler(
            IOptionsMonitor<BankIdAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IBankIdLoginResultProtector loginResultProtector)
            : base(options, logger, encoder, clock)
        {
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

            return Task.FromResult(HandleRequestResult.Success(ticket));
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

        private AuthenticationTicket GetAuthenticationTicket(BankIdLoginResult loginResult, AuthenticationProperties properties)
        {
            var claims = GetClaims(loginResult);
            var identity = new ClaimsIdentity(claims, Scheme.Name, BankIdClaimTypes.Name, BankIdClaimTypes.Role);
            var principal = new ClaimsPrincipal(identity);

            return new AuthenticationTicket(principal, properties, Scheme.Name);
        }

        private IEnumerable<Claim> GetClaims(BankIdLoginResult loginResult)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Parse(loginResult.PersonalIdentityNumber);
            var claims = new List<Claim>
            {
                new Claim(BankIdClaimTypes.Subject, personalIdentityNumber.ToLongString()),

                new Claim(BankIdClaimTypes.Name, loginResult.Name),
                new Claim(BankIdClaimTypes.FamilyName, loginResult.Surname),
                new Claim(BankIdClaimTypes.GivenName, loginResult.GivenName),

                //TODO: Add claim for when the bankid cert expires as not before

                new Claim(BankIdClaimTypes.SwedishPersonalIdentityNumber, personalIdentityNumber.ToShortString())
            };

            AddOptionalClaims(claims, personalIdentityNumber);

            return claims;
        }

        private void AddOptionalClaims(List<Claim> claims, SwedishPersonalIdentityNumber personalIdentityNumber)
        {
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
                var jwtGender = GetJwtGender(personalIdentityNumber.LegalGender);
                claims.Add(new Claim(BankIdClaimTypes.Gender, jwtGender));
            }
        }

        private static string GetJwtGender(SwedishLegalGender gender)
        {
            switch (gender)
            {
                case SwedishLegalGender.Woman:
                    return "female";
                case SwedishLegalGender.Man:
                    return "male";
            }

            return "other";
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            AppendStateCookie(properties);

            var loginUrl = GetLoginUrl();
            Response.Redirect(loginUrl);

            return Task.CompletedTask;
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

        private string GetLoginUrl()
        {
            return $"{Options.BankIdLoginPath}?returnUrl={UrlEncoder.Encode(Options.CallbackPath)}";
        }
    }
}