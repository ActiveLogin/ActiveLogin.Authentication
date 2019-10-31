using System;
using System.Runtime.Serialization;
using System.Text;

namespace ActiveLogin.Authentication.BankId.Api.Models
{
    /// <summary>
    /// Sign request parameters.
    /// </summary>
    [DataContract]
    public class SignRequest
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
        /// <param name="userVisibleData">
        /// The text to be displayed and signed. The text can be formatted using CR, LF and CRLF for new lines.
        /// </param>
        ///
        public SignRequest(string endUserIp, string userVisibleData)
        {
            EndUserIp = endUserIp;
            UserVisibleData = ToBase64EncodedString(userVisibleData);
            Requirement = new Requirement();
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
        /// <param name="userVisibleData">
        /// The text to be displayed and signed. The text can be formatted using CR, LF and CRLF for new lines.
        /// </param>
        /// <param name="userNonVisibleData">
        /// Data not displayed to the user.
        /// </param>
        public SignRequest(string endUserIp, string userVisibleData, byte[] userNonVisibleData)
        {
            EndUserIp = endUserIp;
            UserVisibleData = ToBase64EncodedString(userVisibleData);
            UserNonVisibleData = ToBase64EncodedString(userNonVisibleData);
            Requirement = new Requirement();
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
        /// <param name="userVisibleData">
        /// The text to be displayed and signed. The text can be formatted using CR, LF and CRLF for new lines.
        /// </param>
        /// <param name="personalIdentityNumber">
        /// The personal number of the user. 12 digits, century must be included (YYYYMMDDSSSC).
        /// If the personal number is excluded, the client must be started with the AutoStartToken returned in the response.
        /// </param>
        public SignRequest(string endUserIp, string userVisibleData, string personalIdentityNumber)
        {
            EndUserIp = endUserIp;
            UserVisibleData = ToBase64EncodedString(userVisibleData);
            PersonalIdentityNumber = personalIdentityNumber;
            Requirement = new Requirement();
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
        /// <param name="userVisibleData">
        /// The text to be displayed and signed. The text can be formatted using CR, LF and CRLF for new lines.
        /// </param>
        /// <param name="userNonVisibleData">
        /// Data not displayed to the user.
        /// </param>
        /// <param name="personalIdentityNumber">
        /// The personal number of the user. 12 digits, century must be included (YYYYMMDDSSSC).
        /// If the personal number is excluded, the client must be started with the AutoStartToken returned in the response.
        /// </param>
        public SignRequest(string endUserIp, string userVisibleData, byte[] userNonVisibleData, string personalIdentityNumber)
        {
            EndUserIp = endUserIp;
            UserVisibleData = ToBase64EncodedString(userVisibleData);
            PersonalIdentityNumber = personalIdentityNumber;
            UserNonVisibleData = ToBase64EncodedString(userNonVisibleData);
            Requirement = new Requirement();
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
        /// <param name="userVisibleData">
        /// The text to be displayed and signed. The text can be formatted using CR, LF and CRLF for new lines.
        /// </param>
        /// <param name="userNonVisibleData">
        /// Data not displayed to the user.
        /// </param>
        /// <param name="personalIdentityNumber">
        /// The personal number of the user. 12 digits, century must be included (YYYYMMDDSSSC).
        /// If the personal number is excluded, the client must be started with the AutoStartToken returned in the response.
        /// </param>
        /// <param name="requirement">Requirements on how the auth or sign order must be performed.</param>
        public SignRequest(string endUserIp, string userVisibleData, byte[] userNonVisibleData, string personalIdentityNumber, Requirement requirement)
        {
            EndUserIp = endUserIp;
            UserVisibleData = ToBase64EncodedString(userVisibleData);
            PersonalIdentityNumber = personalIdentityNumber;
            UserNonVisibleData = ToBase64EncodedString(userNonVisibleData);
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
        public string? PersonalIdentityNumber { get; private set; }

        /// <summary>
        /// Requirements on how the auth or sign order must be performed.
        /// </summary>
        [DataMember(Name = "requirement", EmitDefaultValue = false)]
        public Requirement Requirement { get; private set; }

        /// <summary>
        /// The text to be displayed and signed. The text can be formatted using CR, LF and CRLF for new lines.
        /// </summary>
        [DataMember(Name = "userVisibleData")]
        public string UserVisibleData { get; private set; }

        /// <summary>
        /// Data not displayed to the user.
        /// </summary>
        [DataMember(Name = "userNonVisibleData", EmitDefaultValue = false)]
        public string? UserNonVisibleData { get; private set; }

        private static string ToBase64EncodedString(string value)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
        }

        private static string ToBase64EncodedString(byte[] value)
        {
            return Convert.ToBase64String(value);
        }
    }
}