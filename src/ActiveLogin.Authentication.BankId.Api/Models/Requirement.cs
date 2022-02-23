using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models
{
    /// <summary>
    /// Requirements on how the auth or sign order must be performed.
    /// </summary>
    public class Requirement
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="certificatePolicies">The oid in certificate policies in the user certificate. List of String.</param>
        /// <param name="tokenStartRequired">
        /// If set to true, the client must have been started using the AutoStartToken.
        /// To be used if it is important that the BankID App is on the same device as the RP service.
        /// 
        /// If set to false, the client does not need to be started using the autoStartToken.
        /// </param>
        /// <param name="allowFingerprint">
        /// Users of iOS and Android devices may use fingerprint for authentication and signing if the device supports it and the user configured the device to use it.
        /// No other devices are supported at this point.
        /// 
        /// If set to true, the users are allowed to use fingerprint.
        /// If set to false, the users are not allowed to use fingerprint.
        /// </param>
        public Requirement(List<string>? certificatePolicies = null, bool? tokenStartRequired = null, bool? allowFingerprint = null)
        {
            CertificatePolicies = certificatePolicies;
            TokenStartRequired = tokenStartRequired;
            AllowFingerprint = allowFingerprint;
        }

        /// <summary>
        /// The oid in certificate policies in the user certificate. List of String.
        /// </summary>
        [JsonPropertyName("certificatePolicies"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public List<string>? CertificatePolicies { get; private set; }

        /// <summary>
        /// If set to true, the client must have been started using the AutoStartToken.
        /// To be used if it is important that the BankID App is on the same device as the RP service.
        /// 
        /// If set to false, the client does not need to be started using the autoStartToken.
        /// </summary>
        [JsonPropertyName("tokenStartRequired"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool? TokenStartRequired { get; private set; }

        /// <summary>
        /// Users of iOS and Android devices may use fingerprint for authentication and signing if the device supports it and the user configured the device to use it.
        /// No other devices are supported at this point.
        /// 
        /// If set to true, the users are allowed to use fingerprint.
        /// If set to false, the users are not allowed to use fingerprint.
        /// </summary>
        [JsonPropertyName("allowFingerprint"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public bool? AllowFingerprint { get; private set; }
    }
}
