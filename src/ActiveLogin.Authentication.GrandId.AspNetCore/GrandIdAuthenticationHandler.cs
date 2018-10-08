﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
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
                return HandleRequestResult.Fail("Missing sessionId.");
            }

            try
            {
                var sessionResult = await _grandIdApiClient.GetSessionAsync(Options.AuthenticateServiceKey, sessionId);

                var properties = state.AuthenticationProperties;
                var ticket = GetAuthenticationTicket(sessionResult, properties);
                _logger.GrandIdGetSessionSuccess(sessionResult.SessionId);

                return HandleRequestResult.Success(ticket);
            }
            catch (Exception ex)
            {
                _logger.GrandIdGetSessionFailure(sessionId, ex);

                return HandleRequestResult.Fail("Failed to fetch session");
            }
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
                claims.Add(new Claim(GrandIdClaimTypes.Expires, expiresUtc.Value.ToUnixTimeSeconds().ToString("D")));
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
                // Specified in: http://openid.net/specs/openid-connect-core-1_0.html#rfc.section.5.1
                var jwtGender = GetJwtGender(personalIdentityNumber.Gender);
                if (!string.IsNullOrEmpty(jwtGender))
                {
                    claims.Add(new Claim(GrandIdClaimTypes.Gender, jwtGender));
                }
            }

            if (Options.IssueBirthdateClaim)
            {
                // Specified in: http://openid.net/specs/openid-connect-core-1_0.html#rfc.section.5.1
                var jwtBirthdate = GetJwtBirthdate(personalIdentityNumber.DateOfBirth);
                claims.Add(new Claim(GrandIdClaimTypes.Birthdate, jwtBirthdate));
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

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            AppendStateCookie(properties);

            var absoluteReturnUrl = GetAbsoluteUrl(Options.CallbackPath);
            try
            {
                var response = await _grandIdApiClient.FederatedLoginAsync(Options.AuthenticateServiceKey, absoluteReturnUrl);
                _logger.GrandIdAuthSuccess(Options.AuthenticateServiceKey, absoluteReturnUrl, response.SessionId);
                Response.Redirect(response.RedirectUrl);
            }
            catch (Exception ex)
            {
                _logger.GrandIdAuthFailure(Options.AuthenticateServiceKey, absoluteReturnUrl, ex);
                throw;
            }
        }

        private void AppendStateCookie(AuthenticationProperties properties)
        {
            var state = new GrandIdState()
            {
                AuthenticationProperties = properties
            };
            var cookieOptions = Options.StateCookie.Build(Context, Clock.UtcNow);
            var cookieValue = Options.StateDataFormat.Protect(state);

            Response.Cookies.Append(Options.StateCookie.Name, cookieValue, cookieOptions);
        }

        private string GetAbsoluteUrl(string returnUrl)
        {
            var absoluteUri = string.Concat(
                Request.Scheme,
                "://",
                Request.Host.ToUriComponent(),
                Request.PathBase.ToUriComponent());
            return absoluteUri + returnUrl;
        }
    }
}