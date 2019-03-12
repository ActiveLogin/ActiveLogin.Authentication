using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.Common.Serialization;
using ActiveLogin.Identity.Swedish;
using ActiveLogin.Identity.Swedish.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public class BankIdAuthenticationHandler : RemoteAuthenticationHandler<BankIdAuthenticationOptions>
    {
        private readonly ILogger<BankIdAuthenticationHandler> _logger;
        private readonly IBankIdLoginOptionsProtector _loginOptionsProtector;
        private readonly IBankIdLoginResultProtector _loginResultProtector;

        public BankIdAuthenticationHandler(
            IOptionsMonitor<BankIdAuthenticationOptions> options,
            ILoggerFactory loggerFactory,
            UrlEncoder encoder,
            ISystemClock clock,
            ILogger<BankIdAuthenticationHandler> logger,
            IBankIdLoginOptionsProtector loginOptionsProtector,
            IBankIdLoginResultProtector loginResultProtector)
            : base(options, loggerFactory, encoder, clock)
        {
            _logger = logger;
            _loginOptionsProtector = loginOptionsProtector;
            _loginResultProtector = loginResultProtector;
        }

        protected override Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
        {
            BankIdState state = GetStateFromCookie();
            if (state == null)
            {
                return Task.FromResult(HandleRequestResult.Fail("Invalid state cookie."));
            }

            DeleteStateCookie();

            StringValues loginResultProtected = Request.Query["loginResult"];
            if (string.IsNullOrEmpty(loginResultProtected))
            {
                return Task.FromResult(HandleRequestResult.Fail("Missing login result."));
            }

            BankIdLoginResult loginResult = _loginResultProtector.Unprotect(loginResultProtected);
            if (loginResult == null || !loginResult.IsSuccessful)
            {
                return Task.FromResult(HandleRequestResult.Fail("Invalid login result."));
            }

            AuthenticationProperties properties = state.AuthenticationProperties;
            AuthenticationTicket ticket = GetAuthenticationTicket(loginResult, properties);

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

            IEnumerable<Claim> claims = GetClaims(loginResult, expiresUtc);
            var identity = new ClaimsIdentity(claims, Scheme.Name, BankIdClaimTypes.Name, BankIdClaimTypes.Role);
            var principal = new ClaimsPrincipal(identity);

            return new AuthenticationTicket(principal, properties, Scheme.Name);
        }

        private IEnumerable<Claim> GetClaims(BankIdLoginResult loginResult, DateTimeOffset? expiresUtc)
        {
            var personalIdentityNumber = SwedishPersonalIdentityNumber.Parse(loginResult.PersonalIdentityNumber);
            var claims = new List<Claim>
            {
                new Claim(BankIdClaimTypes.Subject, personalIdentityNumber.To12DigitString()),
                new Claim(BankIdClaimTypes.Name, loginResult.Name),
                new Claim(BankIdClaimTypes.FamilyName, loginResult.Surname),
                new Claim(BankIdClaimTypes.GivenName, loginResult.GivenName),
                new Claim(BankIdClaimTypes.SwedishPersonalIdentityNumber, personalIdentityNumber.To10DigitString())
            };

            AddOptionalClaims(claims, personalIdentityNumber, expiresUtc);

            return claims;
        }

        private void AddOptionalClaims(List<Claim> claims, SwedishPersonalIdentityNumber personalIdentityNumber, DateTimeOffset? expiresUtc)
        {
            if (expiresUtc.HasValue)
            {
                claims.Add(new Claim(BankIdClaimTypes.Expires, JwtSerializer.GetExpires(expiresUtc.Value)));
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
                string jwtGender = JwtSerializer.GetGender(personalIdentityNumber.GetGenderHint());
                if (!string.IsNullOrEmpty(jwtGender))
                {
                    claims.Add(new Claim(BankIdClaimTypes.Gender, jwtGender));
                }
            }

            if (Options.IssueBirthdateClaim)
            {
                string jwtBirthdate = JwtSerializer.GetBirthdate(personalIdentityNumber.GetDateOfBirthHint());
                claims.Add(new Claim(BankIdClaimTypes.Birthdate, jwtBirthdate));
            }
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            AppendStateCookie(properties);

            var loginOptions = new BankIdLoginOptions(
                Options.BankIdCertificatePolicies,
                GetSwedishPersonalIdentityNumber(properties),
                Options.BankIdAllowChangingPersonalIdentityNumber,
                Options.BankIdAutoLaunch,
                Options.BankIdAllowBiometric
            );

            string loginUrl = GetLoginUrl(loginOptions);
            Response.Redirect(loginUrl);

            return Task.CompletedTask;
        }

        private static SwedishPersonalIdentityNumber GetSwedishPersonalIdentityNumber( AuthenticationProperties properties)
        {
            if (properties.Items.TryGetValue(BankIdAuthenticationConstants.AuthenticationPropertyItemSwedishPersonalIdentityNumber, out string swedishPersonalIdentityNumber))
            {
                if (!string.IsNullOrWhiteSpace(swedishPersonalIdentityNumber))
                {
                    return SwedishPersonalIdentityNumber.Parse(swedishPersonalIdentityNumber);
                }
            }

            return null;
        }

        private string GetLoginUrl(BankIdLoginOptions loginOptions)
        {
            return $"{Options.LoginPath}" +
                   $"?returnUrl={UrlEncoder.Encode(Options.CallbackPath)}" +
                   $"&loginOptions={UrlEncoder.Encode(_loginOptionsProtector.Protect(loginOptions))}";
        }

        private void AppendStateCookie(AuthenticationProperties properties)
        {
            var state = new BankIdState(properties);
            CookieOptions cookieOptions = Options.StateCookie.Build(Context, Clock.UtcNow);
            string cookieValue = Options.StateDataFormat.Protect(state);

            Response.Cookies.Append(Options.StateCookie.Name, cookieValue, cookieOptions);
        }

        private BankIdState GetStateFromCookie()
        {
            string protectedState = Request.Cookies[Options.StateCookie.Name];
            if (string.IsNullOrEmpty(protectedState))
            {
                return null;
            }

            BankIdState state = Options.StateDataFormat.Unprotect(protectedState);
            return state;
        }

        private void DeleteStateCookie()
        {
            CookieOptions cookieOptions = Options.StateCookie.Build(Context, Clock.UtcNow);
            Response.Cookies.Delete(Options.StateCookie.Name, cookieOptions);
        }
    }
}
