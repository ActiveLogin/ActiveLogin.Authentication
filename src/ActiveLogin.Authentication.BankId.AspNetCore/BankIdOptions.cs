using System;
using System.Collections.Generic;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public class BankIdOptions : RemoteAuthenticationOptions
    {
        private const string DefaultStateCookieName = "__ActiveLogin.BankIdState";

        private CookieBuilder _stateCookieBuilder = new()
        {
            Name = DefaultStateCookieName,
            SecurePolicy = CookieSecurePolicy.SameAsRequest,
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            IsEssential = true
        };

        /// <summary>
        /// The oid in certificate policies in the user certificate. List of String.
        /// </summary>
        public List<string> BankIdCertificatePolicies { get; set; } = new();
        
        /// <summary>
        /// Auto launch the BankID app on the current device.
        /// </summary>
        internal bool BankIdSameDevice { get; set; } = false;

        /// <summary>
        /// Users of iOS and Android devices may use fingerprint or face recognition for authentication if the device supports it and the user configured the device to use it.
        /// </summary>
        public bool BankIdAllowBiometric { get; set; } = true;


        public PathString LoginPath { get; set; } = new($"/{BankIdConstants.AreaName}/Login");
        public TimeSpan? TokenExpiresIn { get; set; } = BankIdDefaults.MaximumSessionLifespan;

        public bool IssueAuthenticationMethodClaim { get; set; } = true;
        public string AuthenticationMethodName { get; set; } = BankIdDefaults.AuthenticationMethodName;

        public bool IssueIdentityProviderClaim { get; set; } = true;
        public string IdentityProviderName { get; set; } = BankIdDefaults.IdentityProviderName;

        public ISecureDataFormat<BankIdState>? StateDataFormat { get; set; }

        public CookieBuilder StateCookie
        {
            get => _stateCookieBuilder;
            set => _stateCookieBuilder = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
