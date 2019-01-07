using System;

namespace ActiveLogin.Authentication.BankId.Api.Models
{
    public static class CertExtensions
    {
        /// <summary>
        /// Start of validity of the users BankID.
        /// </summary>
        public static DateTime GetNotBeforeDateTime(this Cert cert)
        {
            return ParseUnixTimestampMillis(cert.NotBefore);
        }

        /// <summary>
        /// End of validity of the Users BankID.
        /// </summary>
        public static DateTime GetNotAfterDateTime(this Cert cert)
        {
            return ParseUnixTimestampMillis(cert.NotAfter);
        }

        private static DateTime ParseUnixTimestampMillis(string milliseconds)
        {
            return DateTimeFromUnixTimestampMilliseconds(long.Parse(milliseconds));
        }

        private static DateTime DateTimeFromUnixTimestampMilliseconds(long milliseconds)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(milliseconds).DateTime;
        }
    }
}