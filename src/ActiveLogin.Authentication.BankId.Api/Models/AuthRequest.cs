using System.Runtime.Serialization;

namespace ActiveLogin.Authentication.BankId.Api.Models
{
    /// <summary>
    /// Auth request parameters.
    /// </summary>
    [DataContract]
    public class AuthRequest
    {
        /// <summary></summary>
        /// <param name="endUserIp">
        /// The user IP address as seen by RP. IPv4 and IPv6 is allowed.
        /// Note the importance of using the correct IP address.It must be the IP address representing the user agent (the end user device) as seen by the RP.
        /// If there is a proxy for inbound traffic, special considerations may need to be taken to get the correct address.
        ///
        /// In some use cases the IP address is not available, for instance for voice based services.
        /// In this case, the internal representation of those systems IP address is ok to use.
        /// </param>
        public AuthRequest(string endUserIp)
            : this(endUserIp, null)
        {
        }

        /// <summary></summary>
        /// <param name="endUserIp">
        /// The user IP address as seen by RP. IPv4 and IPv6 is allowed.
        /// Note the importance of using the correct IP address.It must be the IP address representing the user agent (the end user device) as seen by the RP.
        /// If there is a proxy for inbound traffic, special considerations may need to be taken to get the correct address.
        ///
        /// In some use cases the IP address is not available, for instance for voice based services.
        /// In this case, the internal representation of those systems IP address is ok to use.
        /// </param>
        /// <param name="personalIdentityNumber">
        /// The personal number of the user. 12 digits, century must be included (YYYYMMDDSSSC).
        /// If the personal number is excluded, the client must be started with the AutoStartToken returned in the response.
        /// </param>
        public AuthRequest(string endUserIp, string personalIdentityNumber)
            : this(endUserIp, personalIdentityNumber, new Requirement())
        {
        }

        /// <summary></summary>
        /// <param name="endUserIp">
        /// The user IP address as seen by RP. IPv4 and IPv6 is allowed.
        /// Note the importance of using the correct IP address.It must be the IP address representing the user agent (the end user device) as seen by the RP.
        /// If there is a proxy for inbound traffic, special considerations may need to be taken to get the correct address.
        ///
        /// In some use cases the IP address is not available, for instance for voice based services.
        /// In this case, the internal representation of those systems IP address is ok to use.
        /// </param>
        /// <param name="personalIdentityNumber">
        /// The personal number of the user. 12 digits, century must be included (YYYYMMDDSSSC).
        /// If the personal number is excluded, the client must be started with the AutoStartToken returned in the response.
        /// </param>
        /// <param name="requirement">Requirements on how the auth or sign order must be performed.</param>
        public AuthRequest(string endUserIp, string personalIdentityNumber, Requirement requirement)
        {
            EndUserIp = endUserIp;
            PersonalIdentityNumber = personalIdentityNumber;
            Requirement = requirement;
        }

        /// <summary>
        /// The user IP address as seen by RP. IPv4 and IPv6 is allowed.
        /// Note the importance of using the correct IP address.It must be the IP address representing the user agent (the end user device) as seen by the RP.
        /// If there is a proxy for inbound traffic, special considerations may need to be taken to get the correct address.
        ///
        /// In some use cases the IP address is not available, for instance for voice based services.
        /// In this case, the internal representation of those systems IP address is ok to use.
        /// </summary>
        [DataMember(Name = "endUserIp")]
        public string EndUserIp { get; private set; }

        /// <summary>
        /// The personal number of the user. 12 digits, century must be included (YYYYMMDDSSSC).
        /// If the personal number is excluded, the client must be started with the AutoStartToken returned in the response.
        /// </summary>
        [DataMember(Name = "personalNumber", EmitDefaultValue = false)]
        public string PersonalIdentityNumber { get; private set; }

        /// <summary>
        /// Requirements on how the auth or sign order must be performed.
        /// </summary>
        [DataMember(Name = "requirement", EmitDefaultValue = false)]
        public Requirement Requirement { get; private set; }
    }
}