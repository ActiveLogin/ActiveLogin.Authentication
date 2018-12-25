using System;
using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models
{
    /// <summary>
    /// Information related to the users certificate (BankID).
    /// </summary>
    [DataContract]
    public class Cert
    {
        internal Cert(string notBefore, string notAfter)
        {
            NotBefore = notBefore;
            NotAfter = notAfter;
        }

        /// <summary>
        /// Start of validity of the users BankID.
        /// </summary>
        [DataMember(Name = "notBefore")]
        public string NotBefore { get; private set; }

        /// <summary>
        /// End of validity of the Users BankID.
        /// </summary>
        [DataMember(Name = "notAfter")]
        public string NotAfter { get; private set; }
    }

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