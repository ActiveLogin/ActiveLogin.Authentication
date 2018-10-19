using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ActiveLogin.Authentication.Common;
using ActiveLogin.Authentication.Common.Serialization;
using ActiveLogin.Authentication.GrandId.Api;
using ActiveLogin.Authentication.GrandId.Api.Models;
using ActiveLogin.Authentication.GrandId.AspNetCore.Models;
using ActiveLogin.Identity.Swedish;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public class GrandIdAuthenticationHandler : RemoteAuthenticationHandler<GrandIdAuthenticationOptions>
    {
        private readonly ILogger<GrandIdAuthenticationHandler> _logger;

        private readonly IGrandIdApiClient _grandIdApiClient;

        public GrandIdAuthenticationHandler(
            IOptionsMonitor<GrandIdAuthenticationOptions> options,
            ILoggerFactory loggerFactory,
            UrlEncoder encoder,
            ISystemClock clock,
            ILogger<GrandIdAuthenticationHandler> logger,
            IGrandIdApiClient grandIdApiClient
            )
            : base(options, loggerFactory, encoder, clock)
        {
            _logger = logger;
            _grandIdApiClient = grandIdApiClient;
        }

        protected override async Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
        {
            var state = GetStateFromCookie();
            if (state == null)
            {
                return HandleRequestResult.Fail("Invalid state cookie.");
            }

            DeleteStateCookie();

            var sessionId = Request.Query["grandidsession"];
            if (string.IsNullOrEmpty(sessionId))
            {
                return HandleRequestResult.Fail("Missing grandidsession from GrandID.");
            }

            try
            {
                var sessionResult = await _grandIdApiClient.GetSessionAsync(Options.GrandIdAuthenticateServiceKey, sessionId);

                var properties = state.AuthenticationProperties;
                var ticket = GetAuthenticationTicket(sessionResult, properties);
                _logger.GrandIdGetSessionSuccess(sessionResult.SessionId);

                return HandleRequestResult.Success(ticket);
            }
            catch (Exception ex)
            {
                _logger.GrandIdGetSessionFailure(sessionId, ex);

                return HandleRequestResult.Fail("Failed to get session from GrandID.");
            }
        }

        private AuthenticationTicket GetAuthenticationTicket(SessionStateResponse loginResult, AuthenticationProperties properties)
        {
            DateTimeOffset? expiresUtc = null;
            if (Options.TokenExpiresIn.HasValue)
            {
                expiresUtc = Clock.UtcNow.Add(Options.TokenExpiresIn.Value);
                properties.ExpiresUtc = expiresUtc;
            }

            var claims = GetClaims(loginResult, expiresUtc);
            var identity = new ClaimsIdentity(claims, Scheme.Name, GrandIdClaimTypes.Name, GrandIdClaimTypes.Role);
            var principal = new ClaimsPrincipal(identity);

            return new AuthenticationTicket(principal, properties, Scheme.Name);
        }

        private IEnumerable<Claim> GetClaims(SessionStateResponse loginResult, DateTimeOffset? expiresUtc)
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

            AddOptionalClaims(claims, personalIdentityNumber, expiresUtc);

            return claims;
        }

        private void AddOptionalClaims(List<Claim> claims, SwedishPersonalIdentityNumber personalIdentityNumber, DateTimeOffset? expiresUtc)
        {
            if (expiresUtc.HasValue)
            {
                claims.Add(new Claim(GrandIdClaimTypes.Expires, JwtSerializer.GetExpires(expiresUtc.Value)));
            }

            if (Options.IssueAuthenticationMethodClaim)
            {
                claims.Add(new Claim(GrandIdClaimTypes.AuthenticationMethod, Options.AuthenticationMethodName));
            }

            if (Options.IssueIdentityProviderClaim)
            {
                claims.Add(new Claim(GrandIdClaimTypes.IdentityProvider, Options.IdentityProviderName));
            }

            if (Options.IssueGenderClaim)
            {
                var jwtGender = JwtSerializer.GetGender(personalIdentityNumber.Gender);
                if (!string.IsNullOrEmpty(jwtGender))
                {
                    claims.Add(new Claim(GrandIdClaimTypes.Gender, jwtGender));
                }
            }

            if (Options.IssueBirthdateClaim)
            {
                var jwtBirthdate = JwtSerializer.GetBirthdate(personalIdentityNumber.DateOfBirth);
                claims.Add(new Claim(GrandIdClaimTypes.Birthdate, jwtBirthdate));
            }
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            AppendStateCookie(properties);

            var absoluteReturnUrl = GetAbsoluteUrl(Options.CallbackPath);
            try
            {
                var response = await _grandIdApiClient.FederatedLoginAsync(Options.GrandIdAuthenticateServiceKey, absoluteReturnUrl);
                _logger.GrandIdAuthSuccess(Options.GrandIdAuthenticateServiceKey, absoluteReturnUrl, response.SessionId);
                Response.Redirect(response.RedirectUrl);
            }
            catch (Exception ex)
            {
                _logger.GrandIdAuthFailure(Options.GrandIdAuthenticateServiceKey, absoluteReturnUrl, ex);
                throw;
            }
        }

        private string GetAbsoluteUrl(string returnUrl)
        {
            var absoluteUri = $"{Request.Scheme}://{Request.Host.ToUriComponent()}{Request.PathBase.ToUriComponent()}";
            return absoluteUri + returnUrl;
        }

        private void AppendStateCookie(AuthenticationProperties properties)
        {
            var state = new GrandIdState(properties);
            var cookieOptions = Options.StateCookie.Build(Context, Clock.UtcNow);
            var cookieValue = Options.StateDataFormat.Protect(state);

            Response.Cookies.Append(Options.StateCookie.Name, cookieValue, cookieOptions);
        }

        private GrandIdState GetStateFromCookie()
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