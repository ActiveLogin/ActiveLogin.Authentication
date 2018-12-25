using System;

namespace ActiveLogin.Authentication.BankId.Api.Models
{
    public static class CertExtensions
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

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
            return UnixEpoch.AddMilliseconds(milliseconds);
        }
    }
}