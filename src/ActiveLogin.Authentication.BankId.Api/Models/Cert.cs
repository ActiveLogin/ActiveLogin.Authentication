using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models
{
    /// <summary>
    /// Information related to the users certificate (BankID).
    /// </summary>
    public class Cert
    {
        public Cert(string notBefore, string notAfter)
        {
            NotBefore = notBefore;
            NotAfter = notAfter;
        }

        /// <summary>
        /// Start of validity of the users BankID. Represented as Unix Timestamp in Milliseconds.
        /// </summary>
        [JsonPropertyName("notBefore")]
        public string NotBefore { get; private set; }

        /// <summary>
        /// End of validity of the Users BankID. Represented as Unix Timestamp in Milliseconds.
        /// </summary>
        [JsonPropertyName("notAfter")]
        public string NotAfter { get; private set; }
    }
}
