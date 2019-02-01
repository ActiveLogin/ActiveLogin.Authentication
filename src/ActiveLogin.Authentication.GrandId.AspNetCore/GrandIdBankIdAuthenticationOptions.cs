using System;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public class GrandIdBankIdAuthenticationOptions : GrandIdAuthenticationOptions
    {
        public override string AuthenticationMethodName { get; set; } = GrandIdAuthenticationDefaults.BankIdAuthenticationMethodName;

        /// <summary>
        /// AuthenticateServiceKey obtained from GrandID (Svensk E-identitet).
        /// </summary>
        public string GrandIdAuthenticateServiceKey { get; set; }

        /// <summary>
        /// Whether or not to issue gender claim based on Swedish Personal Identity Number.
        /// See https://github.com/ActiveLogin/ActiveLogin.Identity for more info and limitations.
        /// </summary>
        public bool IssueGenderClaim { get; set; } = false;

        /// <summary>
        /// Whether or not to issue birthday claim based on Swedish Personal Identity Number.
        /// See https://github.com/ActiveLogin/ActiveLogin.Identity for more info and limitations.
        /// </summary>
        public bool IssueBirthdateClaim { get; set; } = false;

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