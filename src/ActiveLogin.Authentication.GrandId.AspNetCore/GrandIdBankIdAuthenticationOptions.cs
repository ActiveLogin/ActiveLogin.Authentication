using System;
using ActiveLogin.Authentication.GrandId.AspNetCore.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public class GrandIdBankIdAuthenticationOptions : RemoteAuthenticationOptions
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

        /// <summary>
        /// AuthenticateServiceKey obtained from GrandID (Svensk E-identitet).
        /// </summary>
        public string GrandIdAuthenticateServiceKey { get; set; }

        public TimeSpan? TokenExpiresIn { get; set; } = GrandIdAuthenticationDefaults.MaximumSessionLifespan;

        public bool IssueAuthenticationMethodClaim { get; set; } = true;
        public string AuthenticationMethodName { get; set; } = GrandIdAuthenticationDefaults.BankIdAuthenticationMethodName;

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

        public override void Validate()
        {
            base.Validate();

            if (string.IsNullOrWhiteSpace(GrandIdAuthenticateServiceKey))
            {
                throw new ArgumentException($"The '{nameof(GrandIdAuthenticateServiceKey)}' must be provided.'", nameof(GrandIdAuthenticateServiceKey));
            }
        }
    }
}