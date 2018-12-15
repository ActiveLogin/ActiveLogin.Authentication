using System;
using ActiveLogin.Authentication.GrandId.AspNetCore.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public abstract class GrandIdAuthenticationOptions : RemoteAuthenticationOptions
    {
        private const string DefaultStateCookieName = "__ActiveLogin.GrandIdState";

        private CookieBuilder _stateCookieBuilder = new CookieBuilder
        {
            Name = DefaultStateCookieName,
            SecurePolicy = CookieSecurePolicy.SameAsRequest,
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            IsEssential = true
        };

        public TimeSpan? TokenExpiresIn { get; set; } = GrandIdAuthenticationDefaults.MaximumSessionLifespan;

        public bool IssueAuthenticationMethodClaim { get; set; } = true;
        public abstract string AuthenticationMethodName { get; set; }

        public bool IssueIdentityProviderClaim { get; set; } = true;
        public string IdentityProviderName { get; set; } = GrandIdAuthenticationDefaults.IdentityProviderName;

        public ISecureDataFormat<GrandIdState> StateDataFormat { get; set; }

        public CookieBuilder StateCookie
        {
            get => _stateCookieBuilder;
            set => _stateCookieBuilder = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}