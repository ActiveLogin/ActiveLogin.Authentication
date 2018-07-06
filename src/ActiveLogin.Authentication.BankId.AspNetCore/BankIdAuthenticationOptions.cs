using System;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public class BankIdAuthenticationOptions : RemoteAuthenticationOptions
    {
        private const string DefaultStateCookieName = "__BankIdState";

        private CookieBuilder _stateCookieBuilder;

        public BankIdAuthenticationOptions()
        {
            CallbackPath = new PathString("/signin-bankid");
            BankIdLoginPath = new PathString($"/{BankIdAuthenticationConstants.AreaName}/Login");

            AuthenticationMethodName = BankIdAuthenticationDefaults.AuthenticationMethodName;
            IdentityProviderName = BankIdAuthenticationDefaults.IdentityProviderName;

            _stateCookieBuilder = new CookieBuilder()
            {
                Name = DefaultStateCookieName,
                SecurePolicy = CookieSecurePolicy.SameAsRequest,
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                IsEssential = true
            };
        }

        public PathString BankIdLoginPath { get; set; }
        public string AuthenticationMethodName { get; set; }
        public string IdentityProviderName { get; set; }

        public ISecureDataFormat<BankIdState> StateDataFormat { get; set; }

        public CookieBuilder StateCookie
        {
            get => _stateCookieBuilder;
            set => _stateCookieBuilder = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}