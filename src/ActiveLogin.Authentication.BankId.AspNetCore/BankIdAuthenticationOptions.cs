using System;
using System.Collections.Generic;
using ActiveLogin.Authentication.BankId.AspNetCore.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace ActiveLogin.Authentication.BankId.AspNetCore
{
    public class BankIdAuthenticationOptions : RemoteAuthenticationOptions
    {
        private const string DefaultStateCookieName = "__ActiveLogin.BankIdState";

        private CookieBuilder _stateCookieBuilder = new CookieBuilder
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
        public List<string> BankIdCertificatePolicies { get; set; } = new List<string>();

        /// <summary>
        /// Allow the user to set and/or change the personal identity number in the UI.
        /// </summary>
        public bool BankIdAllowChangingPersonalIdentityNumber { get; set; } = true;

        /// <summary>
        /// Auto launch the BankID app on the current device.
        /// </summary>
        public bool BankIdAutoLaunch { get; set; } = false;

        /// <summary>
        /// Users of iOS and Android devices may use fingerprint or face recognition for authentication if the device supports it and the user configured the device to use it.
        /// </summary>
        public bool BankIdAllowBiometric { get; set; } = true;

        public PathString LoginPath { get; set; } = new PathString($"/{BankIdAuthenticationConstants.AreaName}/Login");
        public TimeSpan? TokenExpiresIn { get; set; } = BankIdAuthenticationDefaults.MaximumSessionLifespan;

        public bool IssueAuthenticationMethodClaim { get; set; } = true;
        public string AuthenticationMethodName { get; set; } = BankIdAuthenticationDefaults.AuthenticationMethodName;

        public bool IssueIdentityProviderClaim { get; set; } = true;
        public string IdentityProviderName { get; set; } = BankIdAuthenticationDefaults.IdentityProviderName;

        /// <summary>
        /// Whether or not to issue gender claim based on Swedish Personal Identity Number.
        /// </summary>
        public bool IssueGenderClaim { get; set; } = true;

        /// <summary>
        /// Whether or not to issue birthday claim based on Swedish Personal Identity Number.
        /// </summary>
        public bool IssueBirthdateClaim { get; set; } = true;

        public ISecureDataFormat<BankIdState> StateDataFormat { get; set; }

        public CookieBuilder StateCookie
        {
            get => _stateCookieBuilder;
            set => _stateCookieBuilder = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}