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
        public Cert(DateTime notBefore, DateTime notAfter)
        {
            this.notBefore = UnixTimestampMillisecondsFromDateTime(notBefore).ToString("D");
            this.notAfter = UnixTimestampMillisecondsFromDateTime(notAfter).ToString("D");
        }

        [DataMember(Name = "notBefore")]
        private string notBefore { get; set; }

        /// <summary>
        /// Start of validity of the users BankID.
        /// </summary>
        public DateTime NotBefore => ParseUnixTimestampMillis(notBefore);

        [DataMember(Name = "notAfter")]
        private string notAfter { get; set; }

        /// <summary>
        /// End of validity of the Users BankID.
        /// </summary>
        public DateTime NotAfter => ParseUnixTimestampMillis(notAfter);


        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private static DateTime ParseUnixTimestampMillis(string milliseconds)
        {
            return DateTimeFromUnixTimestampMilliseconds(long.Parse(milliseconds));
        }

        private static DateTime DateTimeFromUnixTimestampMilliseconds(long milliseconds)
        {
            return UnixEpoch.AddMilliseconds(milliseconds);
        }

        private static long UnixTimestampMillisecondsFromDateTime(DateTime dateTime)
        {
            return (long) (dateTime - UnixEpoch).TotalMilliseconds;
        }
    }
}