using System;
using ActiveLogin.Authentication.GrandId.AspNetCore.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public class GrandIdAuthenticationOptions : RemoteAuthenticationOptions
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

        public GrandIdAuthenticationOptions()
        {
            CallbackPath = new PathString("/signin-grandid");
        }

        public PathString GrandIdLoginPath { get; set; } = new PathString($"/{GrandIdAuthenticationConstants.AreaName}/Login");
        public TimeSpan? TokenExpiresIn { get; set; } = TimeSpan.FromSeconds(GrandIdAuthenticationDefaults.MaximumSessionLifespanS);

        public bool IssueAuthenticationMethodClaim { get; set; } = true;
        public string AuthenticationMethodName { get; set; } = GrandIdAuthenticationDefaults.AuthenticationMethodName;

        public bool IssueIdentityProviderClaim { get; set; } = true;
        public string IdentityProviderName { get; set; } = GrandIdAuthenticationDefaults.IdentityProviderName;

        public bool IssueGenderClaim { get; set; } = true;
        public bool IssueBirthdateClaim { get; set; } = true;

        public ISecureDataFormat<GrandIdState> StateDataFormat { get; set; }

        public CookieBuilder StateCookie
        {
            get => _stateCookieBuilder;
            set => _stateCookieBuilder = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}