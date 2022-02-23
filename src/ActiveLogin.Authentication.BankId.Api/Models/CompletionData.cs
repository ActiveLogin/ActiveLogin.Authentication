using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models
{
    public class CompletionData
    {
        public CompletionData(User user, Device device, Cert cert, string signature, string ocspResponse)
        {
            User = user;
            Device = device;
            Cert = cert;
            Signature = signature;
            OcspResponse = ocspResponse;
        }

        /// <summary>
        /// Information related to the user.
        /// </summary>
        [JsonPropertyName("user")]
        public User User { get; private set; }

        /// <summary>
        /// Information related to the device.
        /// </summary>
        [JsonPropertyName("device")]
        public Device Device { get; private set; }

        /// <summary>
        /// Information related to the users certificate (BankID).
        /// </summary>
        [JsonPropertyName("cert")]
        public Cert Cert { get; private set; }

        /// <summary>
        /// The signature. Base64-encoded.
        /// The content of the signature is described in BankID Signature Profile specification.
        /// </summary>
        [JsonPropertyName("signature")]
        public string Signature { get; private set; }

        /// <summary>
        /// The OCSP response. String. Base64-encoded.
        /// The OCSP response is signed by a certificate that has the same issuer as the certificate being verified.
        /// The OSCP response has an extension for Nonce.
        /// </summary>
        [JsonPropertyName("ocspResponse")]
        public string OcspResponse { get; private set; }
    }
}
