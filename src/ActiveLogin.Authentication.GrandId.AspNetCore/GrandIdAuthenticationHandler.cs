using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ActiveLogin.Authentication.Common.Serialization;
using ActiveLogin.Authentication.GrandId.AspNetCore.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public abstract class
        GrandIdAuthenticationHandler<TOptions, TGetSessionResponse> : RemoteAuthenticationHandler<TOptions>
        where TOptions : GrandIdAuthenticationOptions, new()
    {
        protected GrandIdAuthenticationHandler(
            IOptionsMonitor<TOptions> options,
            ILoggerFactory loggerFactory,
            UrlEncoder encoder,
            ISystemClock clock
        )
            : base(options, loggerFactory, encoder, clock)
        {
        }

        protected override async Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
        {
            GrandIdState state = GetStateFromCookie();
            if (state == null)
                return HandleRequestResult.Fail("Invalid state cookie.");

            DeleteStateCookie();

            StringValues sessionId = Request.Query["grandidsession"];
            if (string.IsNullOrEmpty(sessionId))
                return HandleRequestResult.Fail("Missing grandidsession from GrandID.");

            try
            {
                TGetSessionResponse sessionResult = await GetSessionResponseAsync(sessionId);

                AuthenticationProperties properties = state.AuthenticationProperties;
                AuthenticationTicket ticket = GetAuthenticationTicket(sessionResult, properties);

                return HandleRequestResult.Success(ticket);
            }
            catch
            {
                return HandleRequestResult.Fail("Failed to get session from GrandID.");
            }
        }

        protected abstract Task<TGetSessionResponse> GetSessionResponseAsync(string sessionId);

        private AuthenticationTicket GetAuthenticationTicket(TGetSessionResponse loginResult,
            AuthenticationProperties properties)
        {
            DateTimeOffset? expiresUtc = null;
            if (Options.TokenExpiresIn.HasValue)
            {
                expiresUtc = Clock.UtcNow.Add(Options.TokenExpiresIn.Value);
                properties.ExpiresUtc = expiresUtc;
            }

            IEnumerable<Claim> claims = GetAllClaims(loginResult, expiresUtc);
            var identity = new ClaimsIdentity(claims, Scheme.Name, GrandIdClaimTypes.Name, GrandIdClaimTypes.Role);
            var principal = new ClaimsPrincipal(identity);

            return new AuthenticationTicket(principal, properties, Scheme.Name);
        }

        private IEnumerable<Claim> GetAllClaims(TGetSessionResponse loginResult, DateTimeOffset? expiresUtc)
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
                claims.Add(new Claim(GrandIdClaimTypes.Expires, JwtSerializer.GetExpires(expiresUtc.Value)));

            if (Options.IssueAuthenticationMethodClaim)
                claims.Add(new Claim(GrandIdClaimTypes.AuthenticationMethod, Options.AuthenticationMethodName));

            if (Options.IssueIdentityProviderClaim)
                claims.Add(new Claim(GrandIdClaimTypes.IdentityProvider, Options.IdentityProviderName));

            return claims;
        }

        protected abstract IEnumerable<Claim> GetClaims(TGetSessionResponse loginResult);

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            AppendStateCookie(properties);

            string absoluteReturnUrl = GetAbsoluteUrl(Options.CallbackPath);
            string redirectUrl = await GetRedirectUrlAsync(properties, absoluteReturnUrl);

            Response.Redirect(redirectUrl);
        }

        protected abstract Task<string> GetRedirectUrlAsync(AuthenticationProperties properties,
            string absoluteReturnUrl);

        private string GetAbsoluteUrl(string returnUrl)
        {
            string absoluteUri =
                $"{Request.Scheme}://{Request.Host.ToUriComponent()}{Request.PathBase.ToUriComponent()}";
            return absoluteUri + returnUrl;
        }

        private void AppendStateCookie(AuthenticationProperties properties)
        {
            var state = new GrandIdState(properties);
            CookieOptions cookieOptions = Options.StateCookie.Build(Context, Clock.UtcNow);
            string cookieValue = Options.StateDataFormat.Protect(state);

            Response.Cookies.Append(Options.StateCookie.Name, cookieValue, cookieOptions);
        }

        private GrandIdState GetStateFromCookie()
        {
            string protectedState = Request.Cookies[Options.StateCookie.Name];
            if (string.IsNullOrEmpty(protectedState))
                return null;

            GrandIdState state = Options.StateDataFormat.Unprotect(protectedState);
            return state;
        }

        private void DeleteStateCookie()
        {
            CookieOptions cookieOptions = Options.StateCookie.Build(Context, Clock.UtcNow);
            Response.Cookies.Delete(Options.StateCookie.Name, cookieOptions);
        }
    }
}
