using System;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public class BankIdAuthenticationOptions : RemoteAuthenticationOptions
    {
        private const string DefaultStateCookieName = "__BankIdState";

        private CookieBuilder _stateCookieBuilder = new CookieBuilder
        {
            Name = DefaultStateCookieName,
            SecurePolicy = CookieSecurePolicy.SameAsRequest,
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            IsEssential = true
        };

        public BankIdAuthenticationOptions()
        {
            CallbackPath = new PathString("/signin-bankid");
        }

        public PathString BankIdLoginPath { get; set; } = new PathString($"/{BankIdAuthenticationConstants.AreaName}/Login");

        public bool IssueAuthenticationMethodClaim { get; set; } = true;
        public string AuthenticationMethodName { get; set; } = BankIdAuthenticationDefaults.AuthenticationMethodName;

        public bool IssueIdentityProviderClaim { get; set; } = true;
        public string IdentityProviderName { get; set; } = BankIdAuthenticationDefaults.IdentityProviderName;

        public bool IssueGenderClaim { get; set; } = true;

        public ISecureDataFormat<BankIdState> StateDataFormat { get; set; }

        public CookieBuilder StateCookie
        {
            get => _stateCookieBuilder;
            set => _stateCookieBuilder = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}