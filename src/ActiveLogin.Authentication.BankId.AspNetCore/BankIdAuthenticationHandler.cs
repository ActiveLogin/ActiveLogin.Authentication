using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ActiveLogin.Authentication.BankId.AspNetCore.DataProtection;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using ActiveLogin.Authentication.Common;
using ActiveLogin.Authentication.Common.Serialization;
using ActiveLogin.Identity.Swedish;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
                var jwtGender = JwtSerializer.GetGender(personalIdentityNumber.Gender);
                if (!string.IsNullOrEmpty(jwtGender))
                {
                    claims.Add(new Claim(BankIdClaimTypes.Gender, jwtGender));
                }
            }

            if (Options.IssueBirthdateClaim)
            {
                var jwtBirthdate = JwtSerializer.GetBirthdate(personalIdentityNumber.DateOfBirth);
                claims.Add(new Claim(BankIdClaimTypes.Birthdate, jwtBirthdate));
            }
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            AppendStateCookie(properties);

            var loginOptions = new BankIdLoginOptions(
                Options.BankIdCertificatePolicies,
                null, 
                Options.BankIdAllowChangingPersonalIdentityNumber,
                Options.BankIdAutoLaunch,
                Options.BankIdAllowBiometric
            );
            var loginUrl = GetLoginUrl(loginOptions);
            Response.Redirect(loginUrl);

            return Task.CompletedTask;
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
            var cookieOptions = Options.StateCookie.Build(Context, Clock.UtcNow);
            var cookieValue = Options.StateDataFormat.Protect(state);

            Response.Cookies.Append(Options.StateCookie.Name, cookieValue, cookieOptions);
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