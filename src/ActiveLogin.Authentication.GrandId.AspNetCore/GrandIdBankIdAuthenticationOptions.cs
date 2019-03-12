using ActiveLogin.Authentication.GrandId.AspNetCore.Models;

namespace ActiveLogin.Authentication.GrandId.AspNetCore
{
    public class GrandIdBankIdAuthenticationOptions : GrandIdAuthenticationOptions
    {
        public override string AuthenticationMethodName { get; set; } = GrandIdAuthenticationDefaults.BankIdAuthenticationMethodName;

        /// <summary>
        ///     What mode to use GrandID BankID in.
        /// </summary>
        internal GrandIdBankIdMode GrandIdBankIdMode { get; set; }

        /// <summary>
        ///     If set to true, only mobile certificates will be allowed to be used (mobile apps).
        /// </summary>
        public bool RequireMobileBankId { get; } = false;

        /// <summary>
        ///     Whether or not to issue gender claim based on Swedish Personal Identity Number.
        ///     See https://github.com/ActiveLogin/ActiveLogin.Identity for more info and limitations.
        /// </summary>
        public bool IssueGenderClaim { get; set; } = false;

        /// <summary>
        ///     Whether or not to issue birthday claim based on Swedish Personal Identity Number.
        ///     See https://github.com/ActiveLogin/ActiveLogin.Identity for more info and limitations.
        /// </summary>
        public bool IssueBirthdateClaim { get; set; } = false;
    }
}
