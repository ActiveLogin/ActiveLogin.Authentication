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
        public Cert(string notBeforeRaw, string notAfteRaw)
        {
            NotBeforeRaw = notBeforeRaw;
            NotAfterRaw = notAfteRaw;
        }

        [DataMember(Name = "notBefore")]
        public string NotBeforeRaw { get; private set; }

        /// <summary>
        /// Start of validity of the users BankID.
        /// </summary>
        public DateTime NotBefore => ParseUnixTimestampMillis(NotBeforeRaw);

        [DataMember(Name = "notAfter")]
        public string NotAfterRaw { get; private set; }

        /// <summary>
        /// End of validity of the Users BankID.
        /// </summary>
        public DateTime NotAfter => ParseUnixTimestampMillis(NotAfterRaw);


        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

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