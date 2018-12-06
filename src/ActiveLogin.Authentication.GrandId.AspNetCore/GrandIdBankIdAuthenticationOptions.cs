using System;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public class GrandIdBankIdAuthenticationOptions : GrandIdAuthenticationOptions
    {
        /// <summary>
        /// AuthenticateServiceKey obtained from GrandID (Svensk E-identitet).
        /// </summary>
        public string GrandIdAuthenticateServiceKey { get; set; }

        /// <summary>
        /// Whether or not to issue gender claim based on Swedish Personal Identity Number.
        /// </summary>
        public bool IssueGenderClaim { get; set; } = true;

        /// <summary>
        /// Whether or not to issue birthday claim based on Swedish Personal Identity Number.
        /// </summary>
        public bool IssueBirthdateClaim { get; set; } = true;

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