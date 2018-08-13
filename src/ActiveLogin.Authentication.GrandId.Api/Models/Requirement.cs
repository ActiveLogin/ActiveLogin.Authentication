using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.GrandId.Api.Models
{
    /// <summary>
    /// Requirements on how the auth or sign order must be performed.
    /// </summary>
    [DataContract]
    public class Requirement
    {
        /// <summary>
        /// The oid in certificate policies in the user certificate. List of String.
        /// </summary>
        [DataMember(Name = "certificatePolicies", EmitDefaultValue = false)]
        public string CertificatePolicies { get; set; }

        /// <summary>
        /// If set to true, the client must have been started using the AutoStartToken.
        /// To be used if it is important that the BankID App is on the same device as the RP service.
        /// 
        /// If set to false, the client does not need to be started using the autoStartToken.
        /// </summary>
        [DataMember(Name = "autoStartTokenRequired", EmitDefaultValue = false)]
        public bool? AutoStartTokenRequired { get; set; }

        /// <summary>
        /// Users of iOS and Android devices may use fingerprint for authentication and signing if the device supports it and the user configured the device to use it.
        /// No other devices are supported at this point.
        /// 
        /// If set to true, the users are allowed to use fingerprint.
        /// If set to false, the users are not allowed to use fingerprint.
        /// </summary>
        [DataMember(Name = "allowFingerprint", EmitDefaultValue = false)]
        public bool? AllowFingerprint { get; set; }
    }
}