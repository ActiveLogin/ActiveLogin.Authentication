using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ActiveLogin.Authentication.GrandId.Api;
using ActiveLogin.Authentication.GrandId.Api.Models;
using ActiveLogin.Authentication.GrandId.AspNetCore.Models;
using ActiveLogin.Authentication.GrandId.AspNetCore.Serialization;
using ActiveLogin.Identity.Swedish;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public abstract class GrandIdAuthenticationHandler<TOptions, THandler> : RemoteAuthenticationHandler<TOptions> where TOptions : GrandIdAuthenticationOptions, new()
    {
        private readonly ILogger<THandler> _logger;

        private readonly IGrandIdApiClient _grandIdApiClient;

        protected GrandIdAuthenticationHandler(
            IOptionsMonitor<TOptions> options,
            ILoggerFactory loggerFactory,
            UrlEncoder encoder,
            ISystemClock clock,
            ILogger<THandler> logger,
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
                var sessionResult = await _grandIdApiClient.GetSessionAsync(GetGrandIdAuthenticateServiceKey(), sessionId);

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

        protected abstract string GetGrandIdAuthenticateServiceKey();

        private AuthenticationTicket GetAuthenticationTicket(SessionStateResponse loginResult, AuthenticationProperties properties)
        {
            DateTimeOffset? expiresUtc = null;
            if (Options.TokenExpiresIn.HasValue)
            {
                expiresUtc = Clock.UtcNow.Add(Options.TokenExpiresIn.Value);
                properties.ExpiresUtc = expiresUtc;
            }

            var claims = GetAllClaims(loginResult, expiresUtc);
            var identity = new ClaimsIdentity(claims, Scheme.Name, GrandIdClaimTypes.Name, GrandIdClaimTypes.Role);
            var principal = new ClaimsPrincipal(identity);

            return new AuthenticationTicket(principal, properties, Scheme.Name);
        }

        private IEnumerable<Claim> GetAllClaims(SessionStateResponse loginResult, DateTimeOffset? expiresUtc)
        {
            var claims = new List<Claim>();

            claims.AddRange(GetBaseClaims(expiresUtc));
            claims.AddRange(GetClaims(loginResult));

            return claims;
        }

        private IEnumerable<Claim> GetBaseClaims(DateTimeOffset? expiresUtc)
        {
            var claims = new List<Claim>();

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

            return claims;
        }

        protected abstract IEnumerable<Claim> GetClaims(SessionStateResponse loginResult);

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            AppendStateCookie(properties);

            var absoluteReturnUrl = GetAbsoluteUrl(Options.CallbackPath);
            var swedishPersonalIdentityNumber = GetSwedishPersonalIdentityNumber(properties);
            var grandIdAuthenticateServiceKey = GetGrandIdAuthenticateServiceKey();

            try
            {
                var response = await _grandIdApiClient.FederatedLoginAsync(grandIdAuthenticateServiceKey, absoluteReturnUrl, swedishPersonalIdentityNumber?.ToLongString());
                _logger.GrandIdFederatedLoginSuccess(grandIdAuthenticateServiceKey, absoluteReturnUrl, response.SessionId);
                Response.Redirect(response.RedirectUrl);
            }
            catch (Exception ex)
            {
                _logger.GrandIdFederatedLoginFailure(grandIdAuthenticateServiceKey, absoluteReturnUrl, ex);
                throw;
            }
        }

        protected virtual SwedishPersonalIdentityNumber GetSwedishPersonalIdentityNumber(AuthenticationProperties properties)
        {
            return null;
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