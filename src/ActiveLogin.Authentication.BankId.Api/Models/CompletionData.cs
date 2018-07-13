using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models
{
    [DataContract]
    public class CompletionData
    {
        /// <summary>
        /// Information related to the user.
        /// </summary>
        [DataMember(Name = "user")]
        public User User { get; set; }

        /// <summary>
        /// Information related to the device, holds the following child.
        /// </summary>
        [DataMember(Name = "device")]
        public Device Device { get; set; }

        /// <summary>
        /// Information related to the users certificate (BankID).
        /// </summary>
        [DataMember(Name = "cert")]
        public Cert Cert { get; set; }

        /// <summary>
        /// The signature. The content of the signature is described in BankID Signature Profile specification.
        /// </summary>
        [DataMember(Name = "signature")]
        public string Signature { get; set; }

        /// <summary>
        /// The OCSP response. String. Base64-encoded.
        /// The OCSP response is signed by a certificate that has the same issuer as the certificate being verified.
        /// The OSCP response has an extension for Nonce.
        /// </summary>
        [DataMember(Name = "ocspResponse")]
        public string OcspResponse { get; set; }
    }
}